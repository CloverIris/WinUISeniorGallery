using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3.Senior.Controls;

public enum SemanticZoomMode { Detail, Overview }

public sealed class SemanticZoomGroup(string key, string title, IReadOnlyList<object> items)
{
    public string Key { get; } = string.IsNullOrWhiteSpace(key) ? throw new ArgumentException("A group key is required.", nameof(key)) : key;
    public string Title { get; } = string.IsNullOrWhiteSpace(title) ? key : title;
    public IReadOnlyList<object> Items { get; } = items ?? throw new ArgumentNullException(nameof(items));
    public int Count => Items.Count;
}

public sealed class SemanticZoomChangedEventArgs(SemanticZoomMode previous, SemanticZoomMode current, SemanticZoomGroup? group) : EventArgs
{
    public SemanticZoomMode Previous { get; } = previous;
    public SemanticZoomMode Current { get; } = current;
    public SemanticZoomGroup? FocusedGroup { get; } = group;
}

public sealed class SemanticZoomGroupInvokedEventArgs(SemanticZoomGroup group, int index) : EventArgs
{
    public SemanticZoomGroup Group { get; } = group;
    public int Index { get; } = index;
}

/// <summary>
/// Coordinates a detailed collection and a compact group overview without owning the data source.
/// Group projection and display text are supplied by the host.
/// </summary>
public sealed class SemanticZoomView : Control
{
    private readonly ObservableCollection<SemanticZoomGroup> _groups = new();
    private ItemsControl? _detailView;
    private ItemsControl? _overviewView;
    private FrameworkElement? _viewport;
    private bool _internalMode;

    public SemanticZoomView()
    {
        DefaultStyleKey = typeof(SemanticZoomView);
        _groups.CollectionChanged += OnGroupsChanged;
        GroupKeySelector = static item =>
        {
            var text = item?.ToString();
            return string.IsNullOrEmpty(text) ? "#" : text[..1].ToUpperInvariant();
        };
        GroupTitleSelector = static key => key;
        KeyDown += OnKeyDown;
        PointerWheelChanged += OnPointerWheelChanged;
        IsTabStop = true;
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<object>), typeof(SemanticZoomView), new PropertyMetadata(null, OnItemsSourceChanged));
    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(SemanticZoomMode), typeof(SemanticZoomView), new PropertyMetadata(SemanticZoomMode.Detail, OnModeChanged));
    public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(nameof(IsZoomEnabled), typeof(bool), typeof(SemanticZoomView), new PropertyMetadata(true));
    public static readonly DependencyProperty FocusedGroupIndexProperty = DependencyProperty.Register(nameof(FocusedGroupIndex), typeof(int), typeof(SemanticZoomView), new PropertyMetadata(-1, OnFocusedGroupChanged));
    public static readonly DependencyProperty IsReducedMotionProperty = DependencyProperty.Register(nameof(IsReducedMotion), typeof(bool), typeof(SemanticZoomView), new PropertyMetadata(false));

    public IEnumerable<object>? ItemsSource { get => (IEnumerable<object>?)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public SemanticZoomMode Mode { get => (SemanticZoomMode)GetValue(ModeProperty); private set => SetValue(ModeProperty, value); }
    public bool IsZoomEnabled { get => (bool)GetValue(IsZoomEnabledProperty); set => SetValue(IsZoomEnabledProperty, value); }
    public int FocusedGroupIndex { get => (int)GetValue(FocusedGroupIndexProperty); private set => SetValue(FocusedGroupIndexProperty, value); }
    public bool IsReducedMotion { get => (bool)GetValue(IsReducedMotionProperty); set => SetValue(IsReducedMotionProperty, value); }
    public Func<object?, string> GroupKeySelector { get; set; }
    public Func<string, string> GroupTitleSelector { get; set; }
    public IReadOnlyList<SemanticZoomGroup> Groups => _groups;
    public event EventHandler<SemanticZoomChangedEventArgs>? ZoomChanged;
    public event EventHandler<SemanticZoomGroupInvokedEventArgs>? GroupInvoked;

    protected override AutomationPeer OnCreateAutomationPeer() => new SemanticZoomViewAutomationPeer(this);

    public void RebuildGroups()
    {
        var source = ItemsSource?.ToArray() ?? Array.Empty<object>();
        var groups = source.GroupBy(item => NormalizeGroupKey(GroupKeySelector(item)), StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(group => group.Key, StringComparer.CurrentCultureIgnoreCase)
            .Select(group => new SemanticZoomGroup(group.Key, GroupTitleSelector(group.Key), group.ToArray()))
            .ToArray();
        _groups.Clear();
        foreach (var group in groups) _groups.Add(group);
        FocusedGroupIndex = _groups.Count == 0 ? -1 : Math.Clamp(FocusedGroupIndex < 0 ? 0 : FocusedGroupIndex, 0, _groups.Count - 1);
        UpdateVisualState();
    }

    public bool ZoomOut()
    {
        if (!IsZoomEnabled || Mode == SemanticZoomMode.Overview) return false;
        ChangeMode(SemanticZoomMode.Overview);
        return true;
    }

    public bool ZoomIn(int groupIndex = -1)
    {
        if (!IsZoomEnabled || Mode == SemanticZoomMode.Detail) return false;
        if (_groups.Count > 0 && groupIndex >= 0) FocusedGroupIndex = Math.Clamp(groupIndex, 0, _groups.Count - 1);
        ChangeMode(SemanticZoomMode.Detail);
        return true;
    }

    public void InvokeGroup(int index)
    {
        if (index < 0 || index >= _groups.Count) return;
        FocusedGroupIndex = index;
        GroupInvoked?.Invoke(this, new SemanticZoomGroupInvokedEventArgs(_groups[index], index));
        ZoomIn(index);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _detailView = GetTemplateChild("PART_ZoomedInView") as ItemsControl;
        _overviewView = GetTemplateChild("PART_ZoomedOutView") as ItemsControl;
        _viewport = GetTemplateChild("PART_Viewport") as FrameworkElement;
        if (_detailView is null || _overviewView is null) throw new InvalidOperationException("SemanticZoomView template must provide PART_ZoomedInView and PART_ZoomedOutView.");
        _detailView.ItemsSource = ItemsSource;
        _overviewView.ItemsSource = _groups;
        UpdateVisualState();
    }

    private void ChangeMode(SemanticZoomMode mode)
    {
        if (mode == Mode) return;
        var previous = Mode;
        _internalMode = true;
        try { Mode = mode; } finally { _internalMode = false; }
        ZoomChanged?.Invoke(this, new SemanticZoomChangedEventArgs(previous, mode, FocusedGroupIndex >= 0 && FocusedGroupIndex < _groups.Count ? _groups[FocusedGroupIndex] : null));
        UpdateVisualState();
    }
    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (SemanticZoomView)d;
        owner.RebuildGroups();
        if (owner._detailView is not null) owner._detailView.ItemsSource = e.NewValue;
    }
    private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (SemanticZoomView)d;
        if (!owner._internalMode && e.NewValue is SemanticZoomMode mode && e.OldValue is SemanticZoomMode previous && mode != previous)
        {
            owner.ZoomChanged?.Invoke(owner, new SemanticZoomChangedEventArgs(previous, mode, owner.FocusedGroupIndex >= 0 && owner.FocusedGroupIndex < owner._groups.Count ? owner._groups[owner.FocusedGroupIndex] : null));
        }
        owner.UpdateVisualState();
    }
    private static void OnFocusedGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SemanticZoomView)d).UpdateVisualState();
    private void OnGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_overviewView is not null) _overviewView.ItemsSource = _groups;
        UpdateVisualState();
    }
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if ((e.Key == Windows.System.VirtualKey.Add || e.Key == Windows.System.VirtualKey.Enter) && Mode == SemanticZoomMode.Overview) { ZoomIn(FocusedGroupIndex); e.Handled = true; }
        else if ((e.Key == Windows.System.VirtualKey.Subtract || e.Key == Windows.System.VirtualKey.Escape) && Mode == SemanticZoomMode.Detail) { ZoomOut(); e.Handled = true; }
        else if (Mode == SemanticZoomMode.Overview && e.Key == (FlowDirection == FlowDirection.RightToLeft ? Windows.System.VirtualKey.Left : Windows.System.VirtualKey.Right)) { FocusedGroupIndex = Math.Min(_groups.Count - 1, FocusedGroupIndex + 1); e.Handled = true; }
        else if (Mode == SemanticZoomMode.Overview && e.Key == (FlowDirection == FlowDirection.RightToLeft ? Windows.System.VirtualKey.Right : Windows.System.VirtualKey.Left)) { FocusedGroupIndex = Math.Max(0, FocusedGroupIndex - 1); e.Handled = true; }
    }
    private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (!IsZoomEnabled) return;
        var point = e.GetCurrentPoint(this);
        if (point.Properties.MouseWheelDelta < 0) e.Handled = ZoomOut();
        else if (point.Properties.MouseWheelDelta > 0) e.Handled = ZoomIn(FocusedGroupIndex);
    }
    private static string NormalizeGroupKey(string? key) => string.IsNullOrWhiteSpace(key) ? "#" : key.Trim();
    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, Mode.ToString(), !IsReducedMotion);
        AutomationProperties.SetLiveSetting(this, AutomationLiveSetting.Polite);
        if (_viewport is not null) AutomationProperties.SetName(_viewport, Mode == SemanticZoomMode.Detail ? "Detailed view" : "Group overview");
    }
}

internal sealed class SemanticZoomViewAutomationPeer(SemanticZoomView owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;
    protected override string GetClassNameCore() => nameof(SemanticZoomView);
    protected override string GetNameCore() => AutomationProperties.GetName(Owner) is { Length: > 0 } name ? name : "Semantic zoom view";
    protected override string GetHelpTextCore() => Owner is SemanticZoomView zoom ? $"{zoom.Mode}; {zoom.Groups.Count} groups" : base.GetHelpTextCore();
    protected override bool IsKeyboardFocusableCore() => Owner is SemanticZoomView zoom && zoom.IsEnabled && zoom.IsTabStop;
}
