using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Core;
using Windows.System;
using Microsoft.UI.Input;

namespace WinUI3.Senior.Controls;

public enum BottomSheetModality { Modal, Modeless }
public enum BottomSheetPlacement { Auto, Bottom, Side, Center }
public enum BottomSheetDismissReason { Programmatic, Drag, Scrim, Escape, Back, HostUnloaded }
public enum BottomSheetSnapPointKind { Pixels, AvailableFraction, Content }

public sealed record BottomSheetSnapPoint
{
    public BottomSheetSnapPoint(string id, BottomSheetSnapPointKind kind, double value)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Snap point id is required.", nameof(id));
        if (kind == BottomSheetSnapPointKind.Pixels && value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
        if (kind == BottomSheetSnapPointKind.AvailableFraction && (value <= 0 || value > 1)) throw new ArgumentOutOfRangeException(nameof(value));
        Id = id; Kind = kind; Value = value;
    }
    public string Id { get; }
    public BottomSheetSnapPointKind Kind { get; }
    public double Value { get; }
}

public sealed class BottomSheetOpeningEventArgs : EventArgs
{
    public BottomSheetOpeningEventArgs(string? requestedSnapPointId) => RequestedSnapPointId = requestedSnapPointId;
    public string? RequestedSnapPointId { get; }
    public bool Cancel { get; set; }
}

public sealed class BottomSheetClosingEventArgs : EventArgs
{
    public BottomSheetClosingEventArgs(BottomSheetDismissReason reason) => Reason = reason;
    public BottomSheetDismissReason Reason { get; }
    public bool Cancel { get; set; }
}

public sealed class BottomSheetClosedEventArgs : EventArgs
{
    public BottomSheetClosedEventArgs(BottomSheetDismissReason reason) => Reason = reason;
    public BottomSheetDismissReason Reason { get; }
}

public sealed class BottomSheetSnapPointChangedEventArgs : EventArgs
{
    public BottomSheetSnapPointChangedEventArgs(string? previousId, string? currentId)
    {
        PreviousId = previousId;
        CurrentId = currentId;
    }
    public string? PreviousId { get; }
    public string? CurrentId { get; }
}

[TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
[TemplatePart(Name = "PART_Surface", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
[TemplatePart(Name = "PART_Scrim", Type = typeof(UIElement))]
[TemplatePart(Name = "PART_DragHandle", Type = typeof(UIElement))]
/// <summary>A host-owned, modal or modeless surface that can snap to content-aware heights.</summary>
public sealed class BottomSheet : ContentControl
{
    private readonly ObservableCollection<BottomSheetSnapPoint> _snapPoints = new();
    private Grid? _root;
    private UIElement? _scrim;
    private UIElement? _surface;
    private ContentPresenter? _contentPresenter;
    private UIElement? _dragHandle;
    private bool _updatingOpenState;
    private bool _opening;
    private bool _closing;
    private bool _allowClosedTransition;
    private bool _dragging;
    private double _dragStartY;
    private double _dragStartExtent;
    private string? _pendingSnapPointId;
    private DependencyObject? _previousFocusedElement;
    private bool _isRestoringFocus;

    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen), typeof(bool), typeof(BottomSheet), new PropertyMetadata(false, OnIsOpenChanged));
    public static readonly DependencyProperty ModalityProperty = DependencyProperty.Register(
        nameof(Modality), typeof(BottomSheetModality), typeof(BottomSheet), new PropertyMetadata(BottomSheetModality.Modal, OnVisualPropertyChanged));
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(
        nameof(Placement), typeof(BottomSheetPlacement), typeof(BottomSheet), new PropertyMetadata(BottomSheetPlacement.Auto, OnVisualPropertyChanged));
    public static readonly DependencyProperty ActiveSnapPointIdProperty = DependencyProperty.Register(
        nameof(ActiveSnapPointId), typeof(string), typeof(BottomSheet), new PropertyMetadata(null, OnActiveSnapPointChanged));
    public static readonly DependencyProperty IsDragEnabledProperty = DependencyProperty.Register(
        nameof(IsDragEnabled), typeof(bool), typeof(BottomSheet), new PropertyMetadata(true));
    public static readonly DependencyProperty IsDismissOnScrimEnabledProperty = DependencyProperty.Register(
        nameof(IsDismissOnScrimEnabled), typeof(bool), typeof(BottomSheet), new PropertyMetadata(true));
    public static readonly DependencyProperty IsDismissOnEscapeEnabledProperty = DependencyProperty.Register(
        nameof(IsDismissOnEscapeEnabled), typeof(bool), typeof(BottomSheet), new PropertyMetadata(true));
    public static readonly DependencyProperty IsFocusTrapEnabledProperty = DependencyProperty.Register(
        nameof(IsFocusTrapEnabled), typeof(bool), typeof(BottomSheet), new PropertyMetadata(true));
    public static readonly DependencyProperty WideModeThresholdProperty = DependencyProperty.Register(
        nameof(WideModeThreshold), typeof(double), typeof(BottomSheet), new PropertyMetadata(720d, OnVisualPropertyChanged));
    public static readonly DependencyProperty WideModePlacementProperty = DependencyProperty.Register(
        nameof(WideModePlacement), typeof(BottomSheetPlacement), typeof(BottomSheet), new PropertyMetadata(BottomSheetPlacement.Side, OnVisualPropertyChanged));

    public BottomSheet()
    {
        DefaultStyleKey = typeof(BottomSheet);
        _snapPoints.CollectionChanged += OnSnapPointsChanged;
        IsTabStop = true;
        KeyDown += OnKeyDown;
        SizeChanged += (_, _) => UpdateVisualState();
        Unloaded += OnUnloaded;
    }

    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public BottomSheetModality Modality { get => (BottomSheetModality)GetValue(ModalityProperty); set => SetValue(ModalityProperty, value); }
    public BottomSheetPlacement Placement { get => (BottomSheetPlacement)GetValue(PlacementProperty); set => SetValue(PlacementProperty, value); }
    public IReadOnlyList<BottomSheetSnapPoint> SnapPoints => _snapPoints;
    public string? ActiveSnapPointId { get => (string?)GetValue(ActiveSnapPointIdProperty); private set => SetValue(ActiveSnapPointIdProperty, value); }
    public bool IsDragEnabled { get => (bool)GetValue(IsDragEnabledProperty); set => SetValue(IsDragEnabledProperty, value); }
    public bool IsDismissOnScrimEnabled { get => (bool)GetValue(IsDismissOnScrimEnabledProperty); set => SetValue(IsDismissOnScrimEnabledProperty, value); }
    public bool IsDismissOnEscapeEnabled { get => (bool)GetValue(IsDismissOnEscapeEnabledProperty); set => SetValue(IsDismissOnEscapeEnabledProperty, value); }
    public bool IsFocusTrapEnabled { get => (bool)GetValue(IsFocusTrapEnabledProperty); set => SetValue(IsFocusTrapEnabledProperty, value); }
    public double WideModeThreshold { get => (double)GetValue(WideModeThresholdProperty); set => SetValue(WideModeThresholdProperty, value); }
    public BottomSheetPlacement WideModePlacement { get => (BottomSheetPlacement)GetValue(WideModePlacementProperty); set => SetValue(WideModePlacementProperty, value); }
    public BottomSheetPlacement EffectivePlacement => Placement == BottomSheetPlacement.Auto
        ? (ActualWidth < WideModeThreshold ? BottomSheetPlacement.Bottom : WideModePlacement)
        : Placement;
    public event EventHandler<BottomSheetOpeningEventArgs>? Opening;
    public event EventHandler<EventArgs>? Opened;
    public event EventHandler<BottomSheetClosingEventArgs>? Closing;
    public event EventHandler<BottomSheetClosedEventArgs>? Closed;
    public event EventHandler<BottomSheetSnapPointChangedEventArgs>? SnapPointChanged;

    public void Open(string? snapPointId = null)
    {
        if (_opening) return;
        _opening = true;
        var wasOpen = IsOpen;
        var args = new BottomSheetOpeningEventArgs(snapPointId);
        try
        {
            Opening?.Invoke(this, args);
            if (args.Cancel)
            {
                if (!wasOpen)
                {
                    _updatingOpenState = true;
                    try { IsOpen = false; } finally { _updatingOpenState = false; }
                }
                return;
            }
            _pendingSnapPointId = snapPointId;
            if (!wasOpen && IsFocusTrapEnabled)
                _previousFocusedElement = XamlRoot is null ? null : FocusManager.GetFocusedElement(XamlRoot) as DependencyObject;
            if (!IsOpen) IsOpen = true;
            else ApplySnapPoint(snapPointId);
            if (!wasOpen && IsFocusTrapEnabled)
                FocusFirstDescendant();
            if (!wasOpen) Opened?.Invoke(this, EventArgs.Empty);
        }
        finally { _opening = false; }
    }

    public void Close(BottomSheetDismissReason reason = BottomSheetDismissReason.Programmatic)
    {
        if (!IsOpen && !_allowClosedTransition && !_closing) return;
        if (_closing) return;
        _closing = true;
        var args = new BottomSheetClosingEventArgs(reason);
        try
        {
            Closing?.Invoke(this, args);
            if (args.Cancel && reason != BottomSheetDismissReason.HostUnloaded)
            {
                _updatingOpenState = true;
                try { IsOpen = true; } finally { _updatingOpenState = false; }
                return;
            }
            _pendingSnapPointId = null;
            _updatingOpenState = true;
            try { IsOpen = false; } finally { _updatingOpenState = false; }
            Closed?.Invoke(this, new BottomSheetClosedEventArgs(reason));
            RestoreFocus();
        }
        finally { _closing = false; }
    }

    public void SnapTo(string snapPointId)
    {
        if (string.IsNullOrWhiteSpace(snapPointId)) throw new ArgumentException("Snap point id is required.", nameof(snapPointId));
        if (!_snapPoints.Any(p => p.Id == snapPointId)) throw new ArgumentException($"Unknown snap point '{snapPointId}'.", nameof(snapPointId));
        if (!IsOpen) { _pendingSnapPointId = snapPointId; return; }
        ApplySnapPoint(snapPointId);
    }

    public void AddSnapPoint(BottomSheetSnapPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);
        if (_snapPoints.Any(p => p.Id == point.Id)) throw new ArgumentException($"Duplicate snap point '{point.Id}'.", nameof(point));
        _snapPoints.Add(point);
    }

    public void SetSnapPoints(IEnumerable<BottomSheetSnapPoint> points)
    {
        ArgumentNullException.ThrowIfNull(points);
        var replacement = points.ToArray();
        if (replacement.GroupBy(p => p.Id, StringComparer.Ordinal).Any(g => g.Count() > 1)) throw new ArgumentException("Snap point ids must be unique.", nameof(points));
        _snapPoints.Clear();
        foreach (var point in replacement) AddSnapPoint(point);
    }

    public bool RemoveSnapPoint(string id)
    {
        var point = _snapPoints.FirstOrDefault(p => p.Id == id);
        if (point is null) return false;
        var removed = _snapPoints.Remove(point);
        if (removed && ActiveSnapPointId == id) ActiveSnapPointId = _snapPoints.FirstOrDefault()?.Id;
        return removed;
    }

    protected override void OnApplyTemplate()
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate();
        _root = GetTemplateChild("PART_Root") as Grid;
        _scrim = GetTemplateChild("PART_Scrim") as UIElement;
        _surface = GetTemplateChild("PART_Surface") as UIElement;
        _contentPresenter = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
        _dragHandle = GetTemplateChild("PART_DragHandle") as UIElement;
        if (_surface is null || _contentPresenter is null)
            throw new InvalidOperationException("BottomSheet template must provide PART_Surface and PART_ContentPresenter.");
        if (_scrim is not null) _scrim.PointerPressed += OnScrimPointerPressed;
        if (_dragHandle is not null)
        {
            _dragHandle.PointerPressed += OnDragPressed;
            _dragHandle.PointerMoved += OnDragMoved;
            _dragHandle.PointerReleased += OnDragReleased;
            _dragHandle.PointerCanceled += OnDragCanceled;
            _dragHandle.PointerCaptureLost += OnDragCaptureLost;
        }
        UpdateVisualState();
        ApplySnapPoint(_pendingSnapPointId ?? ActiveSnapPointId ?? _snapPoints.FirstOrDefault()?.Id);
        if (IsOpen && IsFocusTrapEnabled)
            FocusFirstDescendant();
    }

    private void DetachTemplateHandlers()
    {
        if (_scrim is not null) _scrim.PointerPressed -= OnScrimPointerPressed;
        if (_dragHandle is not null)
        {
            _dragHandle.PointerPressed -= OnDragPressed;
            _dragHandle.PointerMoved -= OnDragMoved;
            _dragHandle.PointerReleased -= OnDragReleased;
            _dragHandle.PointerCanceled -= OnDragCanceled;
            _dragHandle.PointerCaptureLost -= OnDragCaptureLost;
        }
    }

    private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sheet = (BottomSheet)d;
        if (sheet._updatingOpenState) return;
        if ((bool)e.NewValue) sheet.Open(sheet._pendingSnapPointId);
        else if ((bool)e.OldValue)
        {
            sheet._allowClosedTransition = true;
            try { sheet.Close(BottomSheetDismissReason.Programmatic); } finally { sheet._allowClosedTransition = false; }
        }
        sheet.UpdateVisualState();
    }
    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BottomSheet)d).UpdateVisualState();
    private static void OnActiveSnapPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sheet = (BottomSheet)d;
        if (e.NewValue is string id && !sheet._snapPoints.Any(p => p.Id == id)) throw new ArgumentException($"Unknown snap point '{id}'.");
        sheet.ApplySnapPoint((string?)e.NewValue);
    }
    private void OnSnapPointsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (ActiveSnapPointId is null) ActiveSnapPointId = _snapPoints.FirstOrDefault()?.Id;
        else if (!_snapPoints.Any(p => p.Id == ActiveSnapPointId)) ActiveSnapPointId = _snapPoints.FirstOrDefault()?.Id;
    }
    private void ApplySnapPoint(string? id)
    {
        if (id is null) return;
        if (!_snapPoints.Any(p => p.Id == id)) return;
        var old = ActiveSnapPointId;
        if (old == id) { UpdateVisualState(); return; }
        ActiveSnapPointId = id;
        SnapPointChanged?.Invoke(this, new BottomSheetSnapPointChangedEventArgs(old, id));
        UpdateVisualState();
    }
    private void UpdateVisualState()
    {
        var placement = EffectivePlacement;
        _root?.SetValue(AutomationProperties.LiveSettingProperty, AutomationLiveSetting.Polite);
        if (_surface is FrameworkElement surface)
        {
            // The template stays intentionally neutral; placement is applied here so a
            // host can switch between narrow bottom-sheet and wide side-sheet at runtime.
            switch (placement)
            {
                case BottomSheetPlacement.Side:
                    surface.HorizontalAlignment = FlowDirection == FlowDirection.RightToLeft ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                    surface.VerticalAlignment = VerticalAlignment.Stretch;
                    surface.Width = ResolveExtent();
                    surface.Height = double.NaN;
                    break;
                case BottomSheetPlacement.Center:
                    surface.HorizontalAlignment = HorizontalAlignment.Center;
                    surface.VerticalAlignment = VerticalAlignment.Center;
                    surface.Width = Math.Min(ResolveExtent(), Math.Max(320, ActualWidth * .86));
                    surface.Height = Math.Min(ResolveExtent(), Math.Max(220, ActualHeight * .86));
                    break;
                default:
                    surface.HorizontalAlignment = HorizontalAlignment.Stretch;
                    surface.VerticalAlignment = VerticalAlignment.Bottom;
                    surface.Width = double.NaN;
                    surface.Height = ResolveExtent();
                    break;
            }
        }
        if (_dragHandle is FrameworkElement handle)
        {
            var side = placement == BottomSheetPlacement.Side;
            handle.Width = side ? 6 : 48;
            handle.Height = side ? 48 : 6;
            handle.HorizontalAlignment = HorizontalAlignment.Center;
            handle.VerticalAlignment = VerticalAlignment.Center;
        }
        VisualStateManager.GoToState(this, IsOpen ? "Open" : "Closed", true);
        VisualStateManager.GoToState(this, placement.ToString(), true);
        VisualStateManager.GoToState(this, Modality == BottomSheetModality.Modal ? "Modal" : "Modeless", true);
    }
    private void OnScrimPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (IsDismissOnScrimEnabled && Modality == BottomSheetModality.Modal)
        {
            Close(BottomSheetDismissReason.Scrim);
            e.Handled = true;
        }
    }
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Escape && IsDismissOnEscapeEnabled && IsOpen) { Close(BottomSheetDismissReason.Escape); e.Handled = true; }
        else if (e.Key == VirtualKey.Tab && IsOpen && Modality == BottomSheetModality.Modal && IsFocusTrapEnabled)
        {
            if (CycleFocus(InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down)))
                e.Handled = true;
        }
    }
    private void OnDragPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!IsDragEnabled || !IsOpen) return;
        var point = e.GetCurrentPoint(this).Position;
        _dragging = true;
        _dragStartY = EffectivePlacement == BottomSheetPlacement.Side ? point.X : point.Y;
        _dragStartExtent = ResolveExtent();
        ((UIElement)sender).CapturePointer(e.Pointer);
        e.Handled = true;
    }
    private void OnDragMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_dragging) return;
        var current = e.GetCurrentPoint(this).Position;
        var delta = EffectivePlacement == BottomSheetPlacement.Side
            ? (FlowDirection == FlowDirection.RightToLeft ? current.X - _dragStartY : _dragStartY - current.X)
            : _dragStartY - current.Y;
        var target = _dragStartExtent + delta;
        if (target < 64) { Close(BottomSheetDismissReason.Drag); _dragging = false; return; }
        if (_surface is FrameworkElement surface)
        {
            if (EffectivePlacement == BottomSheetPlacement.Side) surface.Width = target;
            else surface.Height = target;
        }
    }
    private void OnDragReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_dragging) return;
        _dragging = false;
        ((UIElement)sender).ReleasePointerCapture(e.Pointer);
        SnapToNearestExtent();
        e.Handled = true;
    }
    private void OnDragCanceled(object sender, PointerRoutedEventArgs e) => CancelDrag((UIElement)sender);
    private void OnDragCaptureLost(object sender, PointerRoutedEventArgs e) => CancelDrag((UIElement)sender);
    private void CancelDrag(UIElement sender)
    {
        if (!_dragging) return;
        _dragging = false;
        SnapToNearestExtent();
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (IsOpen) Close(BottomSheetDismissReason.HostUnloaded);
        DetachTemplateHandlers();
    }
    private double ResolveExtent()
    {
        var point = _snapPoints.FirstOrDefault(p => p.Id == ActiveSnapPointId);
        var available = EffectivePlacement == BottomSheetPlacement.Side ? ActualWidth : ActualHeight;
        if (point is null) return Math.Max(64, available * .5);
        return point.Kind switch
        {
            BottomSheetSnapPointKind.Pixels => point.Value,
            BottomSheetSnapPointKind.AvailableFraction => available * point.Value,
            BottomSheetSnapPointKind.Content => ResolveContentExtent(available),
            _ => Math.Max(64, available * .5)
        };
    }
    private void SnapToNearestExtent()
    {
        if (_snapPoints.Count == 0) return;
        var current = _surface is FrameworkElement element
            ? (EffectivePlacement == BottomSheetPlacement.Side && !double.IsNaN(element.Width) ? element.Width
                : EffectivePlacement != BottomSheetPlacement.Side && !double.IsNaN(element.Height) ? element.Height : ResolveExtent())
            : ResolveExtent();
        var nearest = _snapPoints.OrderBy(p => Math.Abs(ToPixels(p) - current)).First();
        ApplySnapPoint(nearest.Id);
    }
    private double ToPixels(BottomSheetSnapPoint p)
    {
        var available = EffectivePlacement == BottomSheetPlacement.Side ? ActualWidth : ActualHeight;
        return p.Kind switch
        {
            BottomSheetSnapPointKind.Pixels => p.Value,
            BottomSheetSnapPointKind.AvailableFraction => available * p.Value,
            BottomSheetSnapPointKind.Content => ResolveContentExtent(available),
            _ => available * .5
        };
    }

    private double ResolveContentExtent(double available)
    {
        var content = _surface is FrameworkElement element
            ? EffectivePlacement == BottomSheetPlacement.Side ? element.DesiredSize.Width : element.DesiredSize.Height
            : 0;
        return Math.Clamp(content > 0 ? content : available * .5, 64, Math.Max(64, available));
    }

    private bool CycleFocus(bool reverse)
    {
        if (_surface is null || XamlRoot is null) return false;
        var focusables = new List<Control>();
        CollectFocusable(_surface, focusables);
        if (focusables.Count == 0) return _surface.Focus(FocusState.Keyboard);
        var focused = FocusManager.GetFocusedElement(XamlRoot) as DependencyObject;
        var index = focused is null ? -1 : focusables.FindIndex(c => ReferenceEquals(c, focused) || IsDescendant(c, focused));
        var next = reverse
            ? (index <= 0 ? focusables.Count - 1 : index - 1)
            : (index < 0 || index >= focusables.Count - 1 ? 0 : index + 1);
        return focusables[next].Focus(FocusState.Keyboard);
    }

    private void FocusFirstDescendant()
    {
        if (_surface is null) return;
        var focusables = new List<Control>();
        CollectFocusable(_surface, focusables);
        if (focusables.Count > 0)
            DispatcherQueue?.TryEnqueue(() => focusables[0].Focus(FocusState.Keyboard));
        else
            DispatcherQueue?.TryEnqueue(() => _surface.Focus(FocusState.Programmatic));
    }

    private void RestoreFocus()
    {
        if (_isRestoringFocus || _previousFocusedElement is not Control control || !control.IsEnabled) return;
        _isRestoringFocus = true;
        try { DispatcherQueue?.TryEnqueue(() => control.Focus(FocusState.Programmatic)); }
        finally { _previousFocusedElement = null; _isRestoringFocus = false; }
    }

    private static void CollectFocusable(DependencyObject root, ICollection<Control> result)
    {
        if (root is Control control && control.IsTabStop && control.IsEnabled && control.Visibility == Visibility.Visible)
            result.Add(control);
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            CollectFocusable(VisualTreeHelper.GetChild(root, i), result);
    }

    private static bool IsDescendant(DependencyObject ancestor, DependencyObject candidate)
    {
        var current = candidate;
        while (current is not null)
        {
            if (ReferenceEquals(current, ancestor)) return true;
            current = VisualTreeHelper.GetParent(current);
        }
        return false;
    }
}
