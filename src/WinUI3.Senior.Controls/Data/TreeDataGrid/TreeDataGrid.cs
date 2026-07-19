using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;
using Windows.UI.Core;
using Windows.System;

namespace WinUI3.Senior.Controls;

public enum TreeDataGridSelectionMode
{
    Single,
    Multiple,
    Extended
}

public enum TreeDataGridLoadState
{
    NotLoaded,
    Loading,
    Loaded,
    Failed
}

public enum TreeDataGridSortDirection
{
    None,
    Ascending,
    Descending
}

public sealed class TreeDataGridNode : INotifyPropertyChanged
{
    private bool _isExpanded;
    private TreeDataGridLoadState _loadState;
    private string? _loadError;

    public TreeDataGridNode(object? value, string? key = null, IEnumerable<TreeDataGridNode>? children = null, Func<TreeDataGridNode, CancellationToken, Task<IEnumerable<TreeDataGridNode>>>? childrenProvider = null)
    {
        Value = value;
        Key = string.IsNullOrWhiteSpace(key) ? Guid.NewGuid().ToString("N") : key;
        Children = new ObservableCollection<TreeDataGridNode>(children ?? []);
        ChildrenProvider = childrenProvider;
        _loadState = childrenProvider is null ? TreeDataGridLoadState.Loaded : TreeDataGridLoadState.NotLoaded;
    }

    public string Key { get; }
    public object? Value { get; }
    public ObservableCollection<TreeDataGridNode> Children { get; }
    public Func<TreeDataGridNode, CancellationToken, Task<IEnumerable<TreeDataGridNode>>>? ChildrenProvider { get; }
    public TreeDataGridNode? Parent { get; internal set; }
    public bool HasChildren => Children.Count > 0 || ChildrenProvider is not null;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value) return;
            _isExpanded = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
        }
    }
    public TreeDataGridLoadState LoadState
    {
        get => _loadState;
        internal set
        {
            if (_loadState == value) return;
            _loadState = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadState)));
        }
    }
    public string? LoadError
    {
        get => _loadError;
        internal set
        {
            if (string.Equals(_loadError, value, StringComparison.Ordinal)) return;
            _loadError = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadError)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class TreeDataGridColumn
{
    public TreeDataGridColumn(string key, string header, Func<object?, object?> valueAccessor, double width = double.NaN, bool isSortable = true)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Column key is required.", nameof(key));
        if (string.IsNullOrWhiteSpace(header)) throw new ArgumentException("Column header is required.", nameof(header));
        Key = key;
        Header = header;
        ValueAccessor = valueAccessor ?? throw new ArgumentNullException(nameof(valueAccessor));
        Width = width;
        IsSortable = isSortable;
    }

    public string Key { get; }
    public string Header { get; }
    public Func<object?, object?> ValueAccessor { get; }
    public double Width { get; }
    public bool IsSortable { get; }
    public TreeDataGridSortDirection SortDirection { get; internal set; }
}

public sealed record TreeDataGridRow(TreeDataGridNode Node, int Depth, bool IsLeaf, int VisibleIndex) : EventArgs;

public sealed class TreeDataGridSelectionChangedEventArgs : EventArgs
{
    public TreeDataGridSelectionChangedEventArgs(TreeDataGridNode? selectedItem, IReadOnlyList<TreeDataGridNode> selectedItems)
    {
        SelectedItem = selectedItem;
        SelectedItems = selectedItems;
    }

    public TreeDataGridNode? SelectedItem { get; }
    public IReadOnlyList<TreeDataGridNode> SelectedItems { get; }
}

public sealed class TreeDataGridChildrenLoadFailedEventArgs : EventArgs
{
    public TreeDataGridChildrenLoadFailedEventArgs(TreeDataGridNode node, Exception error)
    {
        Node = node;
        Error = error;
    }

    public TreeDataGridNode Node { get; }
    public Exception Error { get; }
}

public sealed class TreeDataGridCellEditEventArgs : EventArgs
{
    public TreeDataGridCellEditEventArgs(TreeDataGridNode node, TreeDataGridColumn column, object? value)
    {
        Node = node;
        Column = column;
        Value = value;
    }

    public TreeDataGridNode Node { get; }
    public TreeDataGridColumn Column { get; }
    public object? Value { get; }
}

/// <summary>
/// A data-oriented tree grid model. It owns flattening, selection and asynchronous child
/// loading while the host remains responsible for row templates and the data objects.
/// </summary>
public sealed class TreeDataGrid : Control
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(TreeDataGrid), new PropertyMetadata(null, OnItemsSourceChanged));
    public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register(nameof(FilterText), typeof(string), typeof(TreeDataGrid), new PropertyMetadata(string.Empty, OnRowsChanged));
    public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(TreeDataGridSelectionMode), typeof(TreeDataGrid), new PropertyMetadata(TreeDataGridSelectionMode.Single, OnSelectionModeChanged));
    public static readonly DependencyProperty IsEditingEnabledProperty = DependencyProperty.Register(nameof(IsEditingEnabled), typeof(bool), typeof(TreeDataGrid), new PropertyMetadata(false));
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(TreeDataGridNode), typeof(TreeDataGrid), new PropertyMetadata(null));
    public static readonly DependencyProperty RealizationBufferProperty = DependencyProperty.Register(nameof(RealizationBuffer), typeof(int), typeof(TreeDataGrid), new PropertyMetadata(2, OnViewportChanged));

    private readonly ObservableCollection<TreeDataGridNode> _rootNodes = new();
    private readonly ObservableCollection<TreeDataGridRow> _rows = new();
    private readonly List<TreeDataGridRow> _realizedRows = new();
    private readonly ObservableCollection<TreeDataGridNode> _selectedItems = new();
    private CancellationTokenSource? _loadCancellation;
    private TreeDataGridColumn? _sortColumn;

    public TreeDataGrid()
    {
        DefaultStyleKey = typeof(TreeDataGrid);
        _rootNodes.CollectionChanged += (_, _) => RefreshRows();
        _selectedItems.CollectionChanged += (_, _) => SelectionChanged?.Invoke(this, new TreeDataGridSelectionChangedEventArgs(SelectedItem, _selectedItems.ToArray()));
        KeyDown += OnKeyDown;
        Unloaded += (_, _) => CancelPendingLoads();
    }

    public object? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public string FilterText { get => (string?)GetValue(FilterTextProperty) ?? string.Empty; set => SetValue(FilterTextProperty, value); }
    public TreeDataGridSelectionMode SelectionMode { get => (TreeDataGridSelectionMode)GetValue(SelectionModeProperty); set => SetValue(SelectionModeProperty, value); }
    public bool IsEditingEnabled { get => (bool)GetValue(IsEditingEnabledProperty); set => SetValue(IsEditingEnabledProperty, value); }
    public TreeDataGridNode? SelectedItem { get => (TreeDataGridNode?)GetValue(SelectedItemProperty); private set => SetValue(SelectedItemProperty, value); }
    public int RealizationBuffer { get => (int)GetValue(RealizationBufferProperty); set => SetValue(RealizationBufferProperty, Math.Max(0, value)); }
    public ObservableCollection<TreeDataGridColumn> Columns { get; } = new();
    public IReadOnlyList<TreeDataGridNode> SelectedItems => _selectedItems;
    public IReadOnlyList<TreeDataGridRow> VisibleRows => _rows;
    /// <summary>Rows within the host viewport plus <see cref="RealizationBuffer"/> on each side.</summary>
    public IReadOnlyList<TreeDataGridRow> RealizedRows => _realizedRows;
    public int ViewportStartIndex { get; private set; }
    public int ViewportRowCount { get; private set; }
    public TreeDataGridColumn? SortColumn => _sortColumn;

    public event EventHandler<TreeDataGridSelectionChangedEventArgs>? SelectionChanged;
    public event EventHandler<TreeDataGridChildrenLoadFailedEventArgs>? ChildrenLoadFailed;
    public event EventHandler<TreeDataGridCellEditEventArgs>? CellEditCommitted;
    public event EventHandler<TreeDataGridRow>? RowInvoked;

    protected override AutomationPeer OnCreateAutomationPeer() => new TreeDataGridAutomationPeer(this);

    public void SetRootItems(IEnumerable<TreeDataGridNode>? nodes)
    {
        _rootNodes.Clear();
        if (nodes is not null) foreach (var node in nodes) AddNode(_rootNodes, node, null);
        RefreshRows();
    }

    public void RefreshRows()
    {
        _rows.Clear();
        var index = 0;
        foreach (var node in EnumerateMatching(_rootNodes, null)) Flatten(node, 0, ref index);
        UpdateRealizedRows();
        if (SelectedItem is not null && !_rows.Any(row => ReferenceEquals(row.Node, SelectedItem))) ClearSelection();
    }

    /// <summary>Sets the logical viewport; no row/container is created outside the realized range.</summary>
    public void SetViewport(int startIndex, int rowCount)
    {
        ViewportStartIndex = Math.Max(0, startIndex);
        ViewportRowCount = Math.Max(0, rowCount);
        UpdateRealizedRows();
    }

    public async Task<bool> SetExpandedAsync(TreeDataGridNode node, bool expanded, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!node.HasChildren) return false;
        if (expanded && node.ChildrenProvider is not null && (node.LoadState is TreeDataGridLoadState.NotLoaded or TreeDataGridLoadState.Failed))
        {
            await LoadChildrenAsync(node, cancellationToken).ConfigureAwait(true);
            if (node.LoadState == TreeDataGridLoadState.Failed) return false;
        }
        node.IsExpanded = expanded;
        RefreshRows();
        return true;
    }

    public async Task<bool> LoadChildrenAsync(TreeDataGridNode node, CancellationToken cancellationToken = default)
    {
        if (node.ChildrenProvider is null) return true;
        CancelPendingLoads();
        var loadCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _loadCancellation = loadCancellation;
        node.LoadState = TreeDataGridLoadState.Loading;
        node.LoadError = null;
        try
        {
            var children = await node.ChildrenProvider(node, loadCancellation.Token).ConfigureAwait(true);
            node.Children.Clear();
            foreach (var child in children ?? []) AddNode(node.Children, child, node);
            node.LoadState = TreeDataGridLoadState.Loaded;
            RefreshRows();
            return true;
        }
        catch (OperationCanceledException) when (loadCancellation.IsCancellationRequested)
        {
            node.LoadState = TreeDataGridLoadState.NotLoaded;
            return false;
        }
        catch (Exception ex)
        {
            node.LoadState = TreeDataGridLoadState.Failed;
            node.LoadError = ex.Message;
            ChildrenLoadFailed?.Invoke(this, new TreeDataGridChildrenLoadFailedEventArgs(node, ex));
            RefreshRows();
            return false;
        }
        finally
        {
            if (ReferenceEquals(_loadCancellation, loadCancellation)) _loadCancellation = null;
            loadCancellation.Dispose();
        }
    }

    public void ToggleExpanded(TreeDataGridNode node) => _ = SetExpandedAsync(node, !node.IsExpanded);

    public void Select(TreeDataGridNode? node, bool append = false, bool range = false)
    {
        if (node is null) { ClearSelection(); return; }
        var rowIndex = IndexOfRow(node);
        if (range && SelectionMode == TreeDataGridSelectionMode.Extended && SelectedItem is not null)
        {
            var current = IndexOfRow(SelectedItem);
            if (current >= 0 && rowIndex >= 0)
            {
                _selectedItems.Clear();
                foreach (var row in _rows.Skip(Math.Min(current, rowIndex)).Take(Math.Abs(current - rowIndex) + 1)) _selectedItems.Add(row.Node);
                SelectedItem = node;
                return;
            }
        }
        if (!append || SelectionMode == TreeDataGridSelectionMode.Single) _selectedItems.Clear();
        if (!_selectedItems.Contains(node)) _selectedItems.Add(node);
        SelectedItem = node;
    }

    public void ClearSelection()
    {
        _selectedItems.Clear();
        SelectedItem = null;
    }

    /// <summary>Toggles one node in Multiple selection without changing the current anchor.</summary>
    public bool ToggleSelection(TreeDataGridNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (SelectionMode == TreeDataGridSelectionMode.Single || IndexOfRow(node) < 0) return false;
        if (_selectedItems.Contains(node)) _selectedItems.Remove(node);
        else _selectedItems.Add(node);
        SelectedItem = node;
        return true;
    }

    public void MoveSelection(int delta, bool extend = false)
    {
        if (_rows.Count == 0) return;
        var current = SelectedItem is null ? -1 : IndexOfRow(SelectedItem);
        var next = Math.Clamp(current + delta, 0, _rows.Count - 1);
        Select(_rows[next].Node, append: extend, range: extend);
    }

    public bool SortBy(TreeDataGridColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);
        if (!column.IsSortable) return false;
        if (_sortColumn is not null && !ReferenceEquals(_sortColumn, column)) _sortColumn.SortDirection = TreeDataGridSortDirection.None;
        column.SortDirection = column.SortDirection switch { TreeDataGridSortDirection.None => TreeDataGridSortDirection.Ascending, TreeDataGridSortDirection.Ascending => TreeDataGridSortDirection.Descending, _ => TreeDataGridSortDirection.None };
        _sortColumn = column.SortDirection == TreeDataGridSortDirection.None ? null : column;
        SortCollection(_rootNodes);
        RefreshRows();
        return true;
    }

    public bool CommitCellEdit(TreeDataGridNode node, TreeDataGridColumn column, object? value)
    {
        if (!IsEditingEnabled) return false;
        // The model exposes a callback-friendly event. No object mutation is guessed here.
        CellEditCommitted?.Invoke(this, new TreeDataGridCellEditEventArgs(node, column, value));
        return true;
    }

    public void InvokeRow(TreeDataGridNode node)
    {
        var row = _rows.FirstOrDefault(item => ReferenceEquals(item.Node, node));
        if (row is not null) RowInvoked?.Invoke(this, row);
    }

    private void Flatten(TreeDataGridNode node, int depth, ref int index)
    {
        _rows.Add(new TreeDataGridRow(node, depth, !node.HasChildren, index++));
        if (!node.IsExpanded) return;
        foreach (var child in EnumerateMatching(node.Children, node)) Flatten(child, depth + 1, ref index);
    }

    private void UpdateRealizedRows()
    {
        _realizedRows.Clear();
        var start = Math.Max(0, ViewportStartIndex - RealizationBuffer);
        var end = ViewportRowCount <= 0 ? Math.Min(_rows.Count, RealizationBuffer * 2 + 1) : Math.Min(_rows.Count, ViewportStartIndex + ViewportRowCount + RealizationBuffer);
        for (var i = start; i < end; i++) _realizedRows.Add(_rows[i]);
    }

    private IEnumerable<TreeDataGridNode> EnumerateMatching(IEnumerable<TreeDataGridNode> nodes, TreeDataGridNode? parent)
    {
        foreach (var node in nodes)
        {
            node.Parent = parent;
            if (string.IsNullOrWhiteSpace(FilterText) || MatchesFilter(node) || HasMatchingDescendant(node)) yield return node;
        }
    }

    private bool HasMatchingDescendant(TreeDataGridNode node) => node.Children.Any(child => MatchesFilter(child) || HasMatchingDescendant(child));
    private bool MatchesFilter(TreeDataGridNode node) => string.IsNullOrWhiteSpace(FilterText) || node.Value?.ToString()?.Contains(FilterText.Trim(), StringComparison.CurrentCultureIgnoreCase) == true;

    private void SortCollection(IList<TreeDataGridNode> nodes)
    {
        if (_sortColumn is null || _sortColumn.SortDirection == TreeDataGridSortDirection.None) return;
        IEnumerable<TreeDataGridNode> sorted = nodes.OrderBy(node => _sortColumn.ValueAccessor(node.Value), Comparer<object?>.Create(CompareValues));
        if (_sortColumn.SortDirection == TreeDataGridSortDirection.Descending) sorted = sorted.Reverse();
        var copy = sorted.ToList();
        nodes.Clear();
        foreach (var node in copy) { nodes.Add(node); SortCollection(node.Children); }
    }

    private int CompareValues(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return 0;
        if (left is null) return -1;
        if (right is null) return 1;
        if (left is IComparable comparable && left.GetType().IsInstanceOfType(right)) return comparable.CompareTo(right);
        return StringComparer.CurrentCultureIgnoreCase.Compare(Convert.ToString(left), Convert.ToString(right));
    }
    private int IndexOfRow(TreeDataGridNode? node)
    {
        if (node is null) return -1;
        for (var index = 0; index < _rows.Count; index++) if (ReferenceEquals(_rows[index].Node, node)) return index;
        return -1;
    }
    private void CancelPendingLoads() { _loadCancellation?.Cancel(); _loadCancellation?.Dispose(); _loadCancellation = null; }
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var shift = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);
        var control = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
        var collapseKey = FlowDirection == FlowDirection.RightToLeft ? VirtualKey.Right : VirtualKey.Left;
        var expandKey = FlowDirection == FlowDirection.RightToLeft ? VirtualKey.Left : VirtualKey.Right;
        if (e.Key == collapseKey && SelectedItem is not null)
        {
            _ = SetExpandedAsync(SelectedItem, false);
            e.Handled = true;
            return;
        }
        if (e.Key == expandKey && SelectedItem is not null)
        {
            _ = SetExpandedAsync(SelectedItem, true);
            e.Handled = true;
            return;
        }
        switch (e.Key)
        {
            case VirtualKey.Up: MoveSelection(-1, shift); e.Handled = true; break;
            case VirtualKey.Down: MoveSelection(1, shift); e.Handled = true; break;
            case VirtualKey.Enter when SelectedItem is not null: InvokeRow(SelectedItem); e.Handled = true; break;
            case VirtualKey.Space when SelectedItem is not null && control:
                if (ToggleSelection(SelectedItem)) e.Handled = true;
                break;
        }
    }

    private static void AddNode(ICollection<TreeDataGridNode> target, TreeDataGridNode node, TreeDataGridNode? parent)
    {
        node.Parent = parent;
        foreach (var child in node.Children) child.Parent = node;
        target.Add(node);
    }
    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TreeDataGrid)d).SetRootItems(e.NewValue as IEnumerable<TreeDataGridNode>);
    private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var grid = (TreeDataGrid)d;
        if (grid.SelectionMode == TreeDataGridSelectionMode.Single && grid._selectedItems.Count > 1)
        {
            var selected = grid.SelectedItem ?? grid._selectedItems[grid._selectedItems.Count - 1];
            grid._selectedItems.Clear();
            grid._selectedItems.Add(selected);
            grid.SelectedItem = selected;
        }
    }
    private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TreeDataGrid)d).RefreshRows();
    private static void OnViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((TreeDataGrid)d).UpdateRealizedRows();
}

internal sealed class TreeDataGridAutomationPeer : FrameworkElementAutomationPeer
{
    private readonly TreeDataGrid _owner;
    public TreeDataGridAutomationPeer(TreeDataGrid owner) : base(owner) => _owner = owner;
    protected override string GetClassNameCore() => nameof(TreeDataGrid);
    protected override string GetNameCore() => $"{nameof(TreeDataGrid)} ({_owner.VisibleRows.Count} rows)";
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.DataGrid;
}
