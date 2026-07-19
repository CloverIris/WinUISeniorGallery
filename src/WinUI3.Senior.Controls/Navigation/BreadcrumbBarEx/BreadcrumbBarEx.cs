using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3.Senior.Controls;

public sealed class BreadcrumbItem
{
    public BreadcrumbItem(string id, string label, object? value = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("An item id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("An item label is required.", nameof(label));
        Id = id; Label = label; Value = value;
    }
    public string Id { get; }
    public string Label { get; }
    public object? Value { get; }
    public bool IsEnabled { get; set; } = true;
    public bool IsCurrent { get; internal set; }
}

public interface IBreadcrumbEditProvider
{
    bool TryParse(string text, IReadOnlyList<BreadcrumbItem> currentItems, out IReadOnlyList<BreadcrumbItem> items, out string? error);
}

public sealed class BreadcrumbItemInvokedEventArgs(BreadcrumbItem item, int index) : EventArgs
{
    public BreadcrumbItem Item { get; } = item;
    public int Index { get; } = index;
}

public sealed class BreadcrumbEditCommittedEventArgs(IReadOnlyList<BreadcrumbItem> items, string text) : EventArgs
{
    public IReadOnlyList<BreadcrumbItem> Items { get; } = items;
    public string Text { get; } = text;
}

public sealed class BreadcrumbNavigationFailedEventArgs(string text, string reason) : EventArgs
{
    public string Text { get; } = text;
    public string Reason { get; } = reason;
    public bool Handled { get; set; }
}

/// <summary>Editable, keyboard-friendly path navigation with host-owned item values.</summary>
public sealed class BreadcrumbBarEx : Control
{
    private readonly ObservableCollection<BreadcrumbItem> _items = new();
    private ItemsControl? _repeater;
    private TextBox? _editBox;
    private FlyoutBase? _flyout;
    private bool _internalItemsUpdate;
    private INotifyCollectionChanged? _boundCollection;

    public BreadcrumbBarEx()
    {
        DefaultStyleKey = typeof(BreadcrumbBarEx);
        _items.CollectionChanged += OnItemsChanged;
        IsTabStop = true;
        KeyDown += OnKeyDown;
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), typeof(IEnumerable<BreadcrumbItem>), typeof(BreadcrumbBarEx), new PropertyMetadata(null, OnItemsSourceChanged));
    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
        nameof(SelectedIndex), typeof(int), typeof(BreadcrumbBarEx), new PropertyMetadata(-1, OnSelectedIndexChanged));
    public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register(
        nameof(IsEditable), typeof(bool), typeof(BreadcrumbBarEx), new PropertyMetadata(false, OnVisualPropertyChanged));
    public static readonly DependencyProperty EditTextProperty = DependencyProperty.Register(
        nameof(EditText), typeof(string), typeof(BreadcrumbBarEx), new PropertyMetadata(string.Empty, OnEditTextChanged));
    public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register(
        nameof(Separator), typeof(string), typeof(BreadcrumbBarEx), new PropertyMetadata("/"));
    public static readonly DependencyProperty NavigateCommandProperty = DependencyProperty.Register(
        nameof(NavigateCommand), typeof(ICommand), typeof(BreadcrumbBarEx), new PropertyMetadata(null));
    public static readonly DependencyProperty EditProviderProperty = DependencyProperty.Register(
        nameof(EditProvider), typeof(IBreadcrumbEditProvider), typeof(BreadcrumbBarEx), new PropertyMetadata(null));
    public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(
        nameof(IsEditing), typeof(bool), typeof(BreadcrumbBarEx), new PropertyMetadata(false, OnVisualPropertyChanged));
    public static readonly DependencyProperty OverflowThresholdProperty = DependencyProperty.Register(
        nameof(OverflowThreshold), typeof(int), typeof(BreadcrumbBarEx), new PropertyMetadata(6));

    public IEnumerable<BreadcrumbItem>? ItemsSource { get => (IEnumerable<BreadcrumbItem>?)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); private set => SetValue(SelectedIndexProperty, value); }
    public bool IsEditable { get => (bool)GetValue(IsEditableProperty); set => SetValue(IsEditableProperty, value); }
    public string EditText { get => (string)GetValue(EditTextProperty); private set => SetValue(EditTextProperty, value ?? string.Empty); }
    public string Separator { get => (string)GetValue(SeparatorProperty); set => SetValue(SeparatorProperty, value); }
    public ICommand? NavigateCommand { get => (ICommand?)GetValue(NavigateCommandProperty); set => SetValue(NavigateCommandProperty, value); }
    public IBreadcrumbEditProvider? EditProvider { get => (IBreadcrumbEditProvider?)GetValue(EditProviderProperty); set => SetValue(EditProviderProperty, value); }
    public bool IsEditing { get => (bool)GetValue(IsEditingProperty); private set => SetValue(IsEditingProperty, value); }
    public int OverflowThreshold { get => (int)GetValue(OverflowThresholdProperty); set => SetValue(OverflowThresholdProperty, Math.Max(0, value)); }
    public IReadOnlyList<BreadcrumbItem> Items => _items;

    public event EventHandler<BreadcrumbItemInvokedEventArgs>? ItemInvoked;
    public event EventHandler<BreadcrumbEditCommittedEventArgs>? EditCommitted;
    public event EventHandler<BreadcrumbNavigationFailedEventArgs>? NavigationFailed;

    protected override AutomationPeer OnCreateAutomationPeer() => new BreadcrumbBarExAutomationPeer(this);

    public void SetItems(IEnumerable<BreadcrumbItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var replacement = items.ToArray();
        if (replacement.Any(item => item is null)) throw new ArgumentException("Items cannot contain null.", nameof(items));
        _internalItemsUpdate = true;
        try
        {
            _items.Clear();
            foreach (var item in replacement) _items.Add(item);
            ItemsSource = _items;
        }
        finally { _internalItemsUpdate = false; }
        RepairSelection();
    }

    /// <summary>
    /// Returns the stable projection used by compact layouts: the root and
    /// current ancestor stay visible while middle ancestors collapse.
    /// </summary>
    public IReadOnlyList<BreadcrumbItem> GetVisibleItems(int maxItems)
    {
        if (maxItems < 1) throw new ArgumentOutOfRangeException(nameof(maxItems));
        if (_items.Count <= maxItems) return _items.ToArray();
        if (maxItems == 1) return new[] { _items[^1] };
        if (maxItems == 2) return new[] { _items[0], _items[^1] };
        var result = new List<BreadcrumbItem>(maxItems) { _items[0] };
        var tailCount = maxItems - 2;
        result.AddRange(_items.Skip(Math.Max(1, _items.Count - tailCount - 1)).Take(tailCount));
        result.Add(_items[^1]);
        return result;
    }

    public void InvokeItem(int index)
    {
        if (index < 0 || index >= _items.Count) return;
        var item = _items[index];
        if (!item.IsEnabled) return;
        SelectedIndex = index;
        ItemInvoked?.Invoke(this, new BreadcrumbItemInvokedEventArgs(item, index));
        if (NavigateCommand?.CanExecute(item.Value ?? item) == true) NavigateCommand.Execute(item.Value ?? item);
    }

    public void BeginEdit()
    {
        if (!IsEditable || IsEditing) return;
        EditText = string.Join($" {Separator} ", _items.Select(item => item.Label));
        IsEditing = true;
        _editBox?.Focus(FocusState.Programmatic);
        _editBox?.SelectAll();
    }

    public bool CommitEdit()
    {
        if (!IsEditing) return false;
        var text = EditText.Trim();
        if (text.Length == 0) return FailEdit(text, "The breadcrumb text cannot be empty.");
        IReadOnlyList<BreadcrumbItem> parsed;
        if (EditProvider is not null)
        {
            if (!EditProvider.TryParse(text, _items, out parsed!, out var error)) return FailEdit(text, error ?? "The path could not be parsed.");
        }
        else
        {
            var labels = text.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (labels.Length == 0) return FailEdit(text, "The path could not be parsed.");
            parsed = labels.Select(label => new BreadcrumbItem(label, label)).ToArray();
        }
        SetItems(parsed);
        IsEditing = false;
        EditCommitted?.Invoke(this, new BreadcrumbEditCommittedEventArgs(parsed, text));
        return true;
    }

    public void CancelEdit()
    {
        if (!IsEditing) return;
        IsEditing = false;
        EditText = string.Empty;
    }

    protected override void OnApplyTemplate()
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate();
        _repeater = GetTemplateChild("PART_Repeater") as ItemsControl;
        _editBox = GetTemplateChild("PART_EditBox") as TextBox;
        _flyout = GetTemplateChild("PART_Flyout") as FlyoutBase;
        if (_repeater is null) throw new InvalidOperationException("BreadcrumbBarEx template must provide PART_Repeater.");
        _repeater.ItemsSource = _items;
        _repeater.KeyDown += OnRepeaterKeyDown;
        if (_editBox is not null)
        {
            _editBox.Text = EditText;
            _editBox.KeyDown += OnEditBoxKeyDown;
            _editBox.LostFocus += OnEditBoxLostFocus;
        }
        UpdateVisualState();
    }

    private void DetachTemplateHandlers()
    {
        if (_repeater is not null) _repeater.KeyDown -= OnRepeaterKeyDown;
        if (_editBox is not null)
        {
            _editBox.KeyDown -= OnEditBoxKeyDown;
            _editBox.LostFocus -= OnEditBoxLostFocus;
        }
    }

    private bool FailEdit(string text, string reason)
    {
        var args = new BreadcrumbNavigationFailedEventArgs(text, reason);
        NavigationFailed?.Invoke(this, args);
        if (!args.Handled) AutomationProperties.SetHelpText(this, reason);
        return false;
    }
    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (BreadcrumbBarEx)d;
        if (owner._boundCollection is not null)
        {
            owner._boundCollection.CollectionChanged -= owner.OnBoundCollectionChanged;
            owner._boundCollection = null;
        }
        if (owner._internalItemsUpdate) return;
        owner._items.Clear();
        if (e.NewValue is IEnumerable<BreadcrumbItem> source)
        {
            foreach (var item in source)
            {
                if (item is null) throw new ArgumentException("Items cannot contain null.", nameof(ItemsSource));
                owner._items.Add(item);
            }
            if (source is INotifyCollectionChanged observable && !ReferenceEquals(source, owner._items))
            {
                owner._boundCollection = observable;
                observable.CollectionChanged += owner.OnBoundCollectionChanged;
            }
        }
        owner.RepairSelection();
    }
    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BreadcrumbBarEx)d).RepairSelection();
    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BreadcrumbBarEx)d).UpdateVisualState();
    private static void OnEditTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (BreadcrumbBarEx)d;
        if (owner._editBox is not null && owner._editBox.Text != (string)e.NewValue) owner._editBox.Text = (string)e.NewValue;
    }
    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) => RepairSelection();
    private void OnBoundCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_internalItemsUpdate || ItemsSource is not IEnumerable<BreadcrumbItem> source) return;
        _internalItemsUpdate = true;
        try
        {
            _items.Clear();
            foreach (var item in source)
            {
                if (item is null) continue;
                _items.Add(item);
            }
        }
        finally { _internalItemsUpdate = false; }
        RepairSelection();
    }
    private void RepairSelection()
    {
        var selected = Math.Clamp(SelectedIndex, _items.Count == 0 ? -1 : 0, Math.Max(-1, _items.Count - 1));
        if (_items.Count == 0) selected = -1;
        if (selected != SelectedIndex) SetValue(SelectedIndexProperty, selected);
        for (var index = 0; index < _items.Count; index++) _items[index].IsCurrent = index == selected;
        UpdateVisualState();
    }
    private void OnRepeaterKeyDown(object sender, KeyRoutedEventArgs e) => OnKeyDown(sender, e);
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var delta = FlowDirection == FlowDirection.RightToLeft ? -1 : 1;
        if (e.Key == Windows.System.VirtualKey.Right) { MoveSelection(delta); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.Left) { MoveSelection(-delta); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.Enter && SelectedIndex >= 0) { InvokeItem(SelectedIndex); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.F2) { BeginEdit(); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.Escape && IsEditing) { CancelEdit(); e.Handled = true; }
    }
    private void MoveSelection(int delta) { if (_items.Count > 0) SelectedIndex = Math.Clamp((SelectedIndex < 0 ? 0 : SelectedIndex) + delta, 0, _items.Count - 1); }
    private void OnEditBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter) { CommitEdit(); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.Escape) { CancelEdit(); e.Handled = true; }
    }
    private void OnEditBoxLostFocus(object sender, RoutedEventArgs e) { if (IsEditing) CommitEdit(); }
    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, IsEditing ? "Editing" : "Display", true);
        VisualStateManager.GoToState(this, _items.Count > OverflowThreshold && OverflowThreshold > 0 ? "Overflow" : "FullPath", true);
        AutomationProperties.SetLiveSetting(this, AutomationLiveSetting.Polite);
    }
}

internal sealed class BreadcrumbBarExAutomationPeer(BreadcrumbBarEx owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ToolBar;
    protected override string GetClassNameCore() => nameof(BreadcrumbBarEx);
    protected override string GetNameCore() => AutomationProperties.GetName(Owner) is { Length: > 0 } name ? name : "Breadcrumb navigation";
    protected override bool IsKeyboardFocusableCore() => Owner is BreadcrumbBarEx bar && bar.IsEnabled && bar.IsTabStop;
}
