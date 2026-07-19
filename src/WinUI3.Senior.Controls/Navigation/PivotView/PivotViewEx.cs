using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace WinUI3.Senior.Controls;

public enum PivotHeaderMode { Fixed, Scrollable, Compact }
public enum PivotInteractionState { Empty, Idle, Dragging, Settling }

public sealed class PivotItem(string id, string header, object? content = null)
{
    public string Id { get; } = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("A pivot id is required.", nameof(id)) : id;
    public string Header { get; } = string.IsNullOrWhiteSpace(header) ? throw new ArgumentException("A pivot header is required.", nameof(header)) : header;
    public object? Content { get; set; } = content;
    public bool IsEnabled { get; set; } = true;
}

public sealed class PivotSelectionChangedEventArgs(PivotItem? previous, PivotItem? current, int index) : EventArgs
{
    public PivotItem? Previous { get; } = previous;
    public PivotItem? Current { get; } = current;
    public int Index { get; } = index;
}

/// <summary>Lightweight sibling navigation with one committed selection per gesture.</summary>
public sealed class PivotViewEx : Control
{
    private readonly ObservableCollection<PivotItem> _items = new();
    private ItemsControl? _headerRepeater;
    private ContentPresenter? _contentPresenter;
    private FrameworkElement? _indicator;
    private bool _internalSelection;
    private bool _pointerDown;
    private Point _pointerStart;

    public PivotViewEx()
    {
        DefaultStyleKey = typeof(PivotViewEx);
        _items.CollectionChanged += OnItemsChanged;
        IsSwipeEnabled = true;
        HeaderMode = PivotHeaderMode.Scrollable;
        KeyDown += OnKeyDown;
        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
        PointerCanceled += (_, _) => _pointerDown = false;
        IsTabStop = true;
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<PivotItem>), typeof(PivotViewEx), new PropertyMetadata(null, OnItemsSourceChanged));
    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(PivotViewEx), new PropertyMetadata(-1, OnSelectedIndexChanged));
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(PivotItem), typeof(PivotViewEx), new PropertyMetadata(null));
    public static readonly DependencyProperty IsSwipeEnabledProperty = DependencyProperty.Register(nameof(IsSwipeEnabled), typeof(bool), typeof(PivotViewEx), new PropertyMetadata(true));
    public static readonly DependencyProperty HeaderModeProperty = DependencyProperty.Register(nameof(HeaderMode), typeof(PivotHeaderMode), typeof(PivotViewEx), new PropertyMetadata(PivotHeaderMode.Scrollable, OnVisualPropertyChanged));
    public static readonly DependencyProperty SwipeThresholdProperty = DependencyProperty.Register(nameof(SwipeThreshold), typeof(double), typeof(PivotViewEx), new PropertyMetadata(48d));
    public static readonly DependencyProperty InteractionStateProperty = DependencyProperty.Register(nameof(InteractionState), typeof(PivotInteractionState), typeof(PivotViewEx), new PropertyMetadata(PivotInteractionState.Empty, OnVisualPropertyChanged));

    public IEnumerable<PivotItem>? ItemsSource { get => (IEnumerable<PivotItem>?)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); private set => SetValue(SelectedIndexProperty, value); }
    public PivotItem? SelectedItem { get => (PivotItem?)GetValue(SelectedItemProperty); private set => SetValue(SelectedItemProperty, value); }
    public bool IsSwipeEnabled { get => (bool)GetValue(IsSwipeEnabledProperty); set => SetValue(IsSwipeEnabledProperty, value); }
    public PivotHeaderMode HeaderMode { get => (PivotHeaderMode)GetValue(HeaderModeProperty); set => SetValue(HeaderModeProperty, value); }
    public double SwipeThreshold { get => (double)GetValue(SwipeThresholdProperty); set => SetValue(SwipeThresholdProperty, Math.Max(1, value)); }
    public PivotInteractionState InteractionState { get => (PivotInteractionState)GetValue(InteractionStateProperty); private set => SetValue(InteractionStateProperty, value); }
    public IReadOnlyList<PivotItem> Items => _items;
    public event EventHandler<PivotSelectionChangedEventArgs>? SelectionChanged;

    public void SetItems(IEnumerable<PivotItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items.Clear();
        foreach (var item in items) _items.Add(item);
        RepairSelection();
    }

    public bool MoveNext() => MoveBy(1);
    public bool MovePrevious() => MoveBy(-1);
    public bool MoveTo(int index)
    {
        return ApplySelection(index, force: false);
    }

    private bool ApplySelection(int index, bool force)
    {
        if (index < 0 || index >= _items.Count || !_items[index].IsEnabled || (!force && index == SelectedIndex)) return false;
        var previous = SelectedItem;
        _internalSelection = true;
        try { SelectedIndex = index; } finally { _internalSelection = false; }
        SelectedItem = _items[index];
        InteractionState = PivotInteractionState.Settling;
        _contentPresenter?.SetValue(ContentPresenter.ContentProperty, SelectedItem.Content);
        SelectionChanged?.Invoke(this, new PivotSelectionChangedEventArgs(previous, SelectedItem, index));
        InteractionState = PivotInteractionState.Idle;
        return true;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _headerRepeater = GetTemplateChild("PART_HeaderRepeater") as ItemsControl;
        _contentPresenter = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
        _indicator = GetTemplateChild("PART_Indicator") as FrameworkElement;
        if (_headerRepeater is null) throw new InvalidOperationException("PivotViewEx template must provide PART_HeaderRepeater.");
        _headerRepeater.ItemsSource = _items;
        UpdateVisualState();
    }

    private bool MoveBy(int delta)
    {
        if (_items.Count == 0) return false;
        var logicalDelta = FlowDirection == FlowDirection.RightToLeft ? -delta : delta;
        var target = Math.Clamp((SelectedIndex < 0 ? 0 : SelectedIndex) + logicalDelta, 0, _items.Count - 1);
        return MoveTo(target);
    }
    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (PivotViewEx)d;
        owner._items.Clear();
        if (e.NewValue is IEnumerable<PivotItem> source) foreach (var item in source) owner._items.Add(item);
        owner.RepairSelection();
    }
    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (PivotViewEx)d;
        if (!owner._internalSelection && e.NewValue is int index && !owner.ApplySelection(index, force: true)) owner.RepairSelection();
    }
    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PivotViewEx)d).UpdateVisualState();
    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) => RepairSelection();
    private void RepairSelection()
    {
        if (_items.Count == 0)
        {
            SelectedItem = null;
            _internalSelection = true;
            try { SelectedIndex = -1; } finally { _internalSelection = false; }
            InteractionState = PivotInteractionState.Empty;
            return;
        }
        var index = Math.Clamp(SelectedIndex < 0 ? 0 : SelectedIndex, 0, _items.Count - 1);
        while (index < _items.Count && !_items[index].IsEnabled) index++;
        if (index >= _items.Count) index = 0;
        _internalSelection = true;
        try { SelectedIndex = index; } finally { _internalSelection = false; }
        SelectedItem = _items[index];
        InteractionState = PivotInteractionState.Idle;
        _contentPresenter?.SetValue(ContentPresenter.ContentProperty, SelectedItem.Content);
        UpdateVisualState();
    }
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Right) { MoveBy(1); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.Left) { MoveBy(-1); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.Home) { MoveTo(0); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.End) { MoveTo(_items.Count - 1); e.Handled = true; }
    }
    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!IsSwipeEnabled) return;
        _pointerDown = true;
        _pointerStart = e.GetCurrentPoint(this).Position;
        CapturePointer(e.Pointer);
        InteractionState = PivotInteractionState.Dragging;
    }
    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_pointerDown) return;
        _pointerDown = false;
        ReleasePointerCapture(e.Pointer);
        var delta = e.GetCurrentPoint(this).Position.X - _pointerStart.X;
        InteractionState = PivotInteractionState.Idle;
        if (Math.Abs(delta) < SwipeThreshold) return;
        var nextDirection = delta < 0 ? 1 : -1;
        if (FlowDirection == FlowDirection.RightToLeft) nextDirection = -nextDirection;
        MoveBy(nextDirection);
    }
    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, HeaderMode.ToString(), true);
        VisualStateManager.GoToState(this, InteractionState.ToString(), true);
        AutomationProperties.SetLiveSetting(this, AutomationLiveSetting.Polite);
        if (_indicator is not null) AutomationProperties.SetName(_indicator, $"Pivot {Math.Max(0, SelectedIndex) + 1} of {_items.Count}");
    }
}
