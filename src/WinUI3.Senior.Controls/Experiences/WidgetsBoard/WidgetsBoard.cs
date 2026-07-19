using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Controls;

public readonly record struct WidgetBounds(int Column, int Row, int ColumnSpan, int RowSpan)
{
    public int Right => Column + ColumnSpan;
    public int Bottom => Row + RowSpan;
}

public sealed record WidgetBoardItem
{
    public WidgetBoardItem(string id, object? content = null, int column = 0, int row = 0, int columnSpan = 1, int rowSpan = 1,
        bool isPinned = true, bool isCollapsed = false, bool isRefreshable = true, string? title = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("A widget id is required.", nameof(id));
        if (column < 0 || row < 0) throw new ArgumentOutOfRangeException(nameof(column), "Widget coordinates cannot be negative.");
        if (columnSpan < 1 || rowSpan < 1) throw new ArgumentOutOfRangeException(nameof(columnSpan), "Widget spans must be positive.");
        Id = id;
        Content = content;
        Column = column;
        Row = row;
        ColumnSpan = columnSpan;
        RowSpan = rowSpan;
        IsPinned = isPinned;
        IsCollapsed = isCollapsed;
        IsRefreshable = isRefreshable;
        Title = title;
    }

    public string Id { get; init; }
    public object? Content { get; init; }
    public int Column { get; init; }
    public int Row { get; init; }
    public int ColumnSpan { get; init; }
    public int RowSpan { get; init; }
    public bool IsPinned { get; init; }
    public bool IsCollapsed { get; init; }
    public bool IsRefreshable { get; init; }
    public string? Title { get; init; }
    public WidgetBounds Bounds => new(Column, Row, ColumnSpan, RowSpan);
}

public sealed record WidgetBoardSnapshot(IReadOnlyList<WidgetBoardItem> Items, int ColumnCount);

public sealed class WidgetBoardLayoutChangedEventArgs : EventArgs
{
    public WidgetBoardLayoutChangedEventArgs(WidgetBoardItem item, WidgetBounds previous, WidgetBounds current)
    {
        Item = item;
        Previous = previous;
        Current = current;
    }

    public WidgetBoardItem Item { get; }
    public WidgetBounds Previous { get; }
    public WidgetBounds Current { get; }
}

public sealed class WidgetBoardDragEventArgs : EventArgs
{
    public WidgetBoardDragEventArgs(WidgetBoardItem item, WidgetBounds preview, bool accepted)
    {
        Item = item;
        Preview = preview;
        IsAccepted = accepted;
    }

    public WidgetBoardItem Item { get; }
    public WidgetBounds Preview { get; }
    public bool IsAccepted { get; }
}

/// <summary>
/// A deterministic, in-memory widget board. It owns placement and edit-mode state, while the host
/// owns persistence, navigation and window creation. It never moves content into another window.
/// </summary>
public class WidgetsBoard : Control, IDisposable
{
    private readonly Dictionary<string, CancellationTokenSource> _refreshTokens = new(StringComparer.Ordinal);
    private WidgetBoardItem? _draggedItem;
    private WidgetBounds _dragStart;
    private WidgetBounds? _dropPreview;
    private bool _disposed;

    public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register(nameof(ColumnCount), typeof(int), typeof(WidgetsBoard), new PropertyMetadata(12, OnGridPropertyChanged));
    public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(WidgetsBoard), new PropertyMetadata(96d));
    public static readonly DependencyProperty GapProperty = DependencyProperty.Register(nameof(Gap), typeof(double), typeof(WidgetsBoard), new PropertyMetadata(12d));
    public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(nameof(IsEditMode), typeof(bool), typeof(WidgetsBoard), new PropertyMetadata(false));
    public static readonly DependencyProperty IsHostVisibleProperty = DependencyProperty.Register(nameof(IsHostVisible), typeof(bool), typeof(WidgetsBoard), new PropertyMetadata(true, OnHostStateChanged));
    public static readonly DependencyProperty IsHostWindowActiveProperty = DependencyProperty.Register(nameof(IsHostWindowActive), typeof(bool), typeof(WidgetsBoard), new PropertyMetadata(true, OnHostActiveChanged));

    public WidgetsBoard()
    {
        DefaultStyleKey = typeof(WidgetsBoard);
        Items = new ObservableCollection<WidgetBoardItem>();
        Items.CollectionChanged += OnItemsChanged;
    }

    public ObservableCollection<WidgetBoardItem> Items { get; }
    public int ColumnCount { get => (int)GetValue(ColumnCountProperty); set => SetValue(ColumnCountProperty, Math.Max(1, value)); }
    public double RowHeight { get => (double)GetValue(RowHeightProperty); set => SetValue(RowHeightProperty, Math.Max(1, value)); }
    public double Gap { get => (double)GetValue(GapProperty); set => SetValue(GapProperty, Math.Max(0, value)); }
    public bool IsEditMode { get => (bool)GetValue(IsEditModeProperty); set => SetValue(IsEditModeProperty, value); }
    public bool IsHostVisible { get => (bool)GetValue(IsHostVisibleProperty); set => SetValue(IsHostVisibleProperty, value); }
    public bool IsHostWindowActive { get => (bool)GetValue(IsHostWindowActiveProperty); set => SetValue(IsHostWindowActiveProperty, value); }
    public Func<WidgetBoardItem, CancellationToken, ValueTask<WidgetBoardItem?>>? RefreshProvider { get; set; }
    public WidgetBoardItem? DraggedItem => _draggedItem;
    public WidgetBounds? DropPreview => _dropPreview;

    public event EventHandler<WidgetBoardLayoutChangedEventArgs>? LayoutChanged;
    public event EventHandler<WidgetBoardDragEventArgs>? DragPreviewChanged;
    public event EventHandler<WidgetBoardItem>? ItemRemoved;
    public event EventHandler<WidgetBoardItem>? ItemPinChanged;
    public event EventHandler<Exception>? RefreshFailed;
    public event EventHandler<WidgetBoardItem>? RefreshCompleted;

    public void ReplaceItems(IEnumerable<WidgetBoardItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var snapshot = items.ToArray();
        ValidateUniqueIds(snapshot);
        foreach (var item in snapshot) EnsureWithinGrid(item.Bounds);
        Items.Clear();
        foreach (var item in snapshot) Items.Add(item);
        ReflowPinned();
    }

    public bool TryBeginDrag(string id)
    {
        if (_disposed || !IsEditMode || _draggedItem is not null) return false;
        var index = IndexOf(id);
        if (index < 0 || !Items[index].IsPinned) return false;
        _draggedItem = Items[index];
        _dragStart = _draggedItem.Bounds;
        _dropPreview = _dragStart;
        DragPreviewChanged?.Invoke(this, new WidgetBoardDragEventArgs(_draggedItem, _dragStart, true));
        return true;
    }

    public bool PreviewDrop(int column, int row)
    {
        if (_draggedItem is null) return false;
        var preview = NormalizeBounds(new WidgetBounds(column, row, _draggedItem.ColumnSpan, _draggedItem.RowSpan));
        var accepted = CanPlace(_draggedItem.Id, preview);
        _dropPreview = preview;
        DragPreviewChanged?.Invoke(this, new WidgetBoardDragEventArgs(_draggedItem, preview, accepted));
        return accepted;
    }

    public bool CommitDrop()
    {
        if (_draggedItem is null || _dropPreview is null) return false;
        var item = _draggedItem;
        var preview = _dropPreview.Value;
        if (!CanPlace(item.Id, preview)) { CancelDrag(); return false; }
        var previous = item.Bounds;
        UpdateItem(item with { Column = preview.Column, Row = preview.Row });
        ClearDrag();
        LayoutChanged?.Invoke(this, new WidgetBoardLayoutChangedEventArgs(Find(item.Id)!, previous, preview));
        return true;
    }

    public void CancelDrag() => ClearDrag();

    public bool Pin(string id)
    {
        var index = IndexOf(id);
        if (index < 0 || Items[index].IsPinned) return false;
        var item = Items[index];
        var placement = FindFirstAvailable(item.ColumnSpan, item.RowSpan);
        UpdateItem(item with { IsPinned = true, Column = placement.Column, Row = placement.Row });
        ItemPinChanged?.Invoke(this, Find(id)!);
        return true;
    }

    public bool Unpin(string id)
    {
        var index = IndexOf(id);
        if (index < 0 || !Items[index].IsPinned) return false;
        UpdateItem(Items[index] with { IsPinned = false });
        ItemPinChanged?.Invoke(this, Find(id)!);
        return true;
    }

    public bool Remove(string id)
    {
        var index = IndexOf(id);
        if (index < 0) return false;
        var item = Items[index];
        CancelRefresh(id);
        Items.RemoveAt(index);
        if (string.Equals(_draggedItem?.Id, id, StringComparison.Ordinal)) ClearDrag();
        ItemRemoved?.Invoke(this, item);
        return true;
    }

    public bool ToggleCollapsed(string id)
    {
        var index = IndexOf(id);
        if (index < 0) return false;
        UpdateItem(Items[index] with { IsCollapsed = !Items[index].IsCollapsed });
        return true;
    }

    public bool TryResize(string id, int columnSpan, int rowSpan)
    {
        if (_disposed || !IsEditMode || columnSpan < 1 || rowSpan < 1) return false;
        var index = IndexOf(id);
        if (index < 0 || !Items[index].IsPinned) return false;
        columnSpan = Math.Min(columnSpan, ColumnCount);
        var item = Items[index];
        var candidate = NormalizeBounds(new WidgetBounds(item.Column, item.Row, columnSpan, rowSpan));
        if (!CanPlace(id, candidate)) return false;
        var previous = item.Bounds;
        UpdateItem(item with { Column = candidate.Column, Row = candidate.Row, ColumnSpan = candidate.ColumnSpan, RowSpan = candidate.RowSpan });
        LayoutChanged?.Invoke(this, new WidgetBoardLayoutChangedEventArgs(Find(id)!, previous, candidate));
        return true;
    }

    public WidgetBounds GetSuggestedPlacement(int columnSpan = 1, int rowSpan = 1)
    {
        return FindFirstAvailable(Math.Max(1, columnSpan), Math.Max(1, rowSpan));
    }

    public async ValueTask<bool> RefreshAsync(string id, CancellationToken cancellationToken = default)
    {
        if (_disposed || RefreshProvider is null || !IsHostVisible || !IsHostWindowActive) return false;
        var index = IndexOf(id);
        if (index < 0 || !Items[index].IsRefreshable) return false;
        CancelRefresh(id);
        var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _refreshTokens[id] = linked;
        try
        {
            var item = Items[index];
            var replacement = await RefreshProvider(item, linked.Token);
            linked.Token.ThrowIfCancellationRequested();
            if (_disposed || !IsHostVisible || !IsHostWindowActive || IndexOf(id) < 0 || replacement is null) return false;
            // Refresh updates content metadata only. Layout/edit state belongs to the board and
            // cannot be changed by a data provider that may complete out of order.
            var normalized = new WidgetBoardItem(item.Id, replacement.Content, item.Column, item.Row,
                item.ColumnSpan, item.RowSpan, item.IsPinned, item.IsCollapsed,
                item.IsRefreshable, replacement.Title ?? item.Title);
            UpdateItem(normalized);
            RefreshCompleted?.Invoke(this, Find(id)!);
            return true;
        }
        catch (OperationCanceledException) when (linked.IsCancellationRequested) { return false; }
        catch (Exception ex) { if (!_disposed) RefreshFailed?.Invoke(this, ex); return false; }
        finally
        {
            if (_refreshTokens.TryGetValue(id, out var source) && ReferenceEquals(source, linked))
            {
                _refreshTokens.Remove(id);
                source.Dispose();
            }
        }
    }

    public WidgetBoardSnapshot CaptureSnapshot() => new(Items.ToArray(), ColumnCount);

    public void RestoreSnapshot(WidgetBoardSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ColumnCount = snapshot.ColumnCount;
        ReplaceItems(snapshot.Items);
    }

    public void ReflowPinned()
    {
        var pinned = Items.Where(item => item.IsPinned).ToArray();
        foreach (var item in pinned)
        {
            var destination = FindFirstAvailable(item.ColumnSpan, item.RowSpan, item.Id);
            if (destination != item.Bounds) UpdateItem(item with { Column = destination.Column, Row = destination.Row });
        }
    }

    public bool CanPlace(string id, WidgetBounds bounds)
    {
        if (bounds.Column < 0 || bounds.Row < 0 || bounds.ColumnSpan < 1 || bounds.RowSpan < 1 || bounds.Right > ColumnCount) return false;
        return Items.Where(item => item.IsPinned && !string.Equals(item.Id, id, StringComparison.Ordinal)).All(item => !Overlaps(item.Bounds, bounds));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var token in _refreshTokens.Values) token.Cancel();
        foreach (var token in _refreshTokens.Values) token.Dispose();
        _refreshTokens.Clear();
        Items.CollectionChanged -= OnItemsChanged;
        ClearDrag();
    }

    private static bool Overlaps(WidgetBounds a, WidgetBounds b) => a.Column < b.Right && b.Column < a.Right && a.Row < b.Bottom && b.Row < a.Bottom;
    private WidgetBounds NormalizeBounds(WidgetBounds bounds) => new(Math.Clamp(bounds.Column, 0, Math.Max(0, ColumnCount - bounds.ColumnSpan)), Math.Max(0, bounds.Row), bounds.ColumnSpan, bounds.RowSpan);
    private WidgetBounds FindFirstAvailable(int columnSpan, int rowSpan, string? exceptId = null)
    {
        columnSpan = Math.Min(Math.Max(1, columnSpan), ColumnCount);
        for (var row = 0; row < 10000; row++)
            for (var column = 0; column <= ColumnCount - columnSpan; column++)
            {
                var candidate = new WidgetBounds(column, row, columnSpan, rowSpan);
                if (CanPlace(exceptId ?? string.Empty, candidate)) return candidate;
            }
        return new WidgetBounds(0, 0, columnSpan, rowSpan);
    }

    private int IndexOf(string id)
    {
        for (var index = 0; index < Items.Count; index++) if (string.Equals(Items[index].Id, id, StringComparison.Ordinal)) return index;
        return -1;
    }

    private WidgetBoardItem? Find(string id) => IndexOf(id) is var index && index >= 0 ? Items[index] : null;
    private void UpdateItem(WidgetBoardItem item) { var index = IndexOf(item.Id); if (index >= 0) Items[index] = item; }
    private void EnsureWithinGrid(WidgetBounds bounds) { if (bounds.Right > ColumnCount) throw new ArgumentException("A widget does not fit in the current column count."); }

    private void ClearDrag() { _draggedItem = null; _dropPreview = null; }
    private void CancelRefresh(string id) { if (_refreshTokens.Remove(id, out var token)) { token.Cancel(); token.Dispose(); } }
    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) { if (!_disposed) InvalidateMeasure(); }
    private static void OnGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WidgetsBoard)d).ReflowPinned();
    private static void OnHostStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WidgetsBoard)d).CancelRefreshesWhenInactive();
    private static void OnHostActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WidgetsBoard)d).CancelRefreshesWhenInactive();

    private void CancelRefreshesWhenInactive()
    {
        if (IsHostVisible && IsHostWindowActive) return;
        foreach (var token in _refreshTokens.Values.ToArray()) token.Cancel();
    }

    private static void ValidateUniqueIds(IEnumerable<WidgetBoardItem> items)
    {
        var values = items.ToArray();
        if (values.Select(static item => item.Id).Distinct(StringComparer.Ordinal).Count() != values.Length) throw new ArgumentException("Widget ids must be unique.", nameof(items));
    }
}

/// <summary>Singular alias used by hosts that refer to one board as a WidgetBoard.</summary>
public sealed class WidgetBoard : WidgetsBoard
{
    public WidgetBoard() => DefaultStyleKey = typeof(WidgetBoard);
}
