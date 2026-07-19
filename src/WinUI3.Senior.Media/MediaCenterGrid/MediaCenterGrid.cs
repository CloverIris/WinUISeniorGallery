using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace WinUI3.Senior.Media;

/// <summary>Focus-friendly poster grid for ten-foot media browsing.</summary>
[TemplatePart(Name = "PART_ListView", Type = typeof(ListView))]
public sealed class MediaCenterGrid : ListView
{
    public static readonly DependencyProperty FocusScaleProperty = DependencyProperty.Register(
        nameof(FocusScale), typeof(double), typeof(MediaCenterGrid), new PropertyMetadata(1.08, OnFocusVisualPropertyChanged));
    public static readonly DependencyProperty IsFocusScaleEnabledProperty = DependencyProperty.Register(
        nameof(IsFocusScaleEnabled), typeof(bool), typeof(MediaCenterGrid), new PropertyMetadata(true, OnFocusVisualPropertyChanged));
    public static readonly DependencyProperty PosterAspectRatioProperty = DependencyProperty.Register(
        nameof(PosterAspectRatio), typeof(double), typeof(MediaCenterGrid), new PropertyMetadata(0.6667, OnLayoutPropertyChanged));
    public static readonly DependencyProperty MinimumCardWidthProperty = DependencyProperty.Register(
        nameof(MinimumCardWidth), typeof(double), typeof(MediaCenterGrid), new PropertyMetadata(180d, OnLayoutPropertyChanged));
    public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
        nameof(Columns), typeof(int), typeof(MediaCenterGrid), new PropertyMetadata(6, OnLayoutPropertyChanged));
    public static readonly DependencyProperty IsWrapNavigationEnabledProperty = DependencyProperty.Register(
        nameof(IsWrapNavigationEnabled), typeof(bool), typeof(MediaCenterGrid), new PropertyMetadata(true));
    public static readonly DependencyProperty IsTenFootModeProperty = DependencyProperty.Register(
        nameof(IsTenFootMode), typeof(bool), typeof(MediaCenterGrid), new PropertyMetadata(false, OnFocusVisualPropertyChanged));

    public MediaCenterGrid()
    {
        DefaultStyleKey = typeof(MediaCenterGrid);
        SelectionMode = ListViewSelectionMode.Single;
        IsItemClickEnabled = true;
        ItemClick += OnItemClick;
        KeyDown += OnKeyDown;
        SelectionChanged += OnSelectionChanged;
        ContainerContentChanging += OnContainerContentChanging;
        SizeChanged += (_, _) => ApplyCardLayout();
    }

    public double FocusScale
    {
        get => (double)GetValue(FocusScaleProperty);
        set => SetValue(FocusScaleProperty, value);
    }

    public bool IsFocusScaleEnabled
    {
        get => (bool)GetValue(IsFocusScaleEnabledProperty);
        set => SetValue(IsFocusScaleEnabledProperty, value);
    }

    public double PosterAspectRatio
    {
        get => (double)GetValue(PosterAspectRatioProperty);
        set => SetValue(PosterAspectRatioProperty, value);
    }

    public double MinimumCardWidth
    {
        get => (double)GetValue(MinimumCardWidthProperty);
        set => SetValue(MinimumCardWidthProperty, value);
    }

    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, Math.Clamp(value, 1, 20));
    }

    public bool IsWrapNavigationEnabled
    {
        get => (bool)GetValue(IsWrapNavigationEnabledProperty);
        set => SetValue(IsWrapNavigationEnabledProperty, value);
    }

    public bool IsTenFootMode
    {
        get => (bool)GetValue(IsTenFootModeProperty);
        set => SetValue(IsTenFootModeProperty, value);
    }

    public int FocusedIndex => SelectedIndex;

    public event EventHandler<MediaCenterItemInvokedEventArgs>? ItemInvoked;
    public event EventHandler<MediaCenterFocusChangedEventArgs>? FocusChanged;

    /// <summary>Invokes the selected item without synthesising a pointer event.</summary>
    public bool InvokeSelected(MediaCenterInputDeviceKind inputDeviceKind = MediaCenterInputDeviceKind.Keyboard)
    {
        if (SelectedIndex < 0 || SelectedIndex >= Items.Count) return false;
        RaiseItemInvoked(SelectedItem, SelectedIndex, inputDeviceKind);
        return true;
    }

    /// <summary>Moves focus using ten-foot directional semantics and optional wrapping.</summary>
    public bool MoveFocus(MediaCenterNavigationDirection direction)
    {
        if (Items.Count == 0) return false;
        var columns = Math.Max(1, Columns);
        var current = SelectedIndex < 0 ? 0 : SelectedIndex;
        var target = direction switch
        {
            MediaCenterNavigationDirection.Left => current - 1,
            MediaCenterNavigationDirection.Right => current + 1,
            MediaCenterNavigationDirection.Up => current - columns,
            MediaCenterNavigationDirection.Down => current + columns,
            _ => current,
        };

        if (IsWrapNavigationEnabled)
        {
            if (direction is MediaCenterNavigationDirection.Left or MediaCenterNavigationDirection.Right)
                target = (target % Items.Count + Items.Count) % Items.Count;
            else if (target < 0) target = Items.Count - 1;
            else if (target >= Items.Count) target = 0;
        }

        if (target < 0 || target >= Items.Count) return false;
        SelectedIndex = target;
        ScrollIntoView(SelectedItem, ScrollIntoViewAlignment.Default);
        return true;
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
        if (SelectedIndex >= Items.Count)
        {
            SelectedIndex = Items.Count - 1;
        }
        FocusChanged?.Invoke(this, new MediaCenterFocusChangedEventArgs(SelectedIndex, SelectedItem));
        ApplyFocusVisuals();
    }

    private void OnItemClick(object sender, ItemClickEventArgs args)
    {
        var index = Items.IndexOf(args.ClickedItem);
        RaiseItemInvoked(args.ClickedItem, index, MediaCenterInputDeviceKind.Unknown);
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        ApplyFocusVisuals();
        FocusChanged?.Invoke(this, new MediaCenterFocusChangedEventArgs(SelectedIndex, SelectedItem));
    }

    private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (!args.InRecycleQueue) ApplyFocusVisual(args.ItemIndex);
        if (!args.InRecycleQueue) ApplyCardLayout(args.ItemIndex);
    }

    private static void OnFocusVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MediaCenterGrid)d).ApplyFocusVisuals();
    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MediaCenterGrid)d).ApplyCardLayout();

    private void ApplyFocusVisuals()
    {
        for (var index = 0; index < Items.Count; index++) ApplyFocusVisual(index);
    }

    private void ApplyFocusVisual(int index)
    {
        if (index < 0 || index >= Items.Count) return;
        if (ContainerFromIndex(index) is not ListViewItem container) return;
        var scale = IsFocusScaleEnabled && index == SelectedIndex ? Math.Clamp(FocusScale * (IsTenFootMode ? 1.04 : 1), 1, 1.5) : 1;
        container.RenderTransformOrigin = new Windows.Foundation.Point(.5, .5);
        container.RenderTransform = new Microsoft.UI.Xaml.Media.ScaleTransform { ScaleX = scale, ScaleY = scale };
        Canvas.SetZIndex(container, index == SelectedIndex ? 1 : 0);
    }

    private void ApplyCardLayout(int? index = null)
    {
        var columns = Math.Max(1, Columns);
        var available = ActualWidth > 0 ? ActualWidth : double.NaN;
        var width = double.IsFinite(available)
            ? Math.Max(MinimumCardWidth, (available - (columns - 1) * 12) / columns)
            : Math.Max(1, MinimumCardWidth);
        var ratio = double.IsFinite(PosterAspectRatio) && PosterAspectRatio > 0 ? PosterAspectRatio : 0.6667;
        void Apply(int itemIndex)
        {
            if (ContainerFromIndex(itemIndex) is not ListViewItem container) return;
            container.MinWidth = width;
            container.Width = width;
            container.MinHeight = width / ratio;
        }
        if (index is { } value) Apply(value);
        else for (var i = 0; i < Items.Count; i++) Apply(i);
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs args)
    {
        if (args.Key is VirtualKey.Enter or VirtualKey.Space)
        {
            args.Handled = InvokeSelected(MediaCenterInputDeviceKind.Keyboard);
            return;
        }

        var direction = args.Key switch
        {
            VirtualKey.Left => FlowDirection == FlowDirection.RightToLeft ? MediaCenterNavigationDirection.Right : MediaCenterNavigationDirection.Left,
            VirtualKey.Right => FlowDirection == FlowDirection.RightToLeft ? MediaCenterNavigationDirection.Left : MediaCenterNavigationDirection.Right,
            VirtualKey.Up => MediaCenterNavigationDirection.Up,
            VirtualKey.Down => MediaCenterNavigationDirection.Down,
            _ => (MediaCenterNavigationDirection?)null,
        };
        if (direction is { } value) args.Handled = MoveFocus(value);
    }

    private void RaiseItemInvoked(object? item, int index, MediaCenterInputDeviceKind inputDeviceKind) =>
        ItemInvoked?.Invoke(this, new MediaCenterItemInvokedEventArgs(item, index, inputDeviceKind));
}

public enum MediaCenterInputDeviceKind
{
    Unknown,
    Mouse,
    Touch,
    Keyboard,
    GameController,
}

public enum MediaCenterNavigationDirection
{
    Left,
    Right,
    Up,
    Down,
}

public sealed class MediaCenterItemInvokedEventArgs(object? item, int index, MediaCenterInputDeviceKind inputDeviceKind = MediaCenterInputDeviceKind.Unknown) : EventArgs
{
    public object? Item { get; } = item;
    public int Index { get; } = index;
    public MediaCenterInputDeviceKind InputDeviceKind { get; } = inputDeviceKind;
}

public sealed class MediaCenterFocusChangedEventArgs(int index, object? item) : EventArgs
{
    public int Index { get; } = index;
    public object? Item { get; } = item;
}
