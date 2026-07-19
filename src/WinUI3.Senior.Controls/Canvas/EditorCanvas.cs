using System.Collections.Immutable;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using WinUI3.Senior.Core;

namespace WinUI3.Senior.Controls;

public enum EditorCanvasTool
{
    Select,
    Pan,
    Rectangle,
    Text,
    Ink,
}

public sealed class EditorCanvasObjectInvokedEventArgs(CanvasObject item, CanvasPoint worldPoint, bool isDoubleTap) : EventArgs
{
    public CanvasObject Item { get; } = item ?? throw new ArgumentNullException(nameof(item));
    public CanvasPoint WorldPoint { get; } = worldPoint;
    public bool IsDoubleTap { get; } = isDoubleTap;
}

public sealed class EditorCanvasSelectionChangedEventArgs(IReadOnlySet<string> selectedIds, bool isUserInitiated) : EventArgs
{
    public IReadOnlySet<string> SelectedIds { get; } = selectedIds ?? throw new ArgumentNullException(nameof(selectedIds));
    public bool IsUserInitiated { get; } = isUserInitiated;
}

/// <summary>
/// A host-neutral WinUI canvas surface. Core owns the document and viewport math;
/// this control owns hit testing, input capture and rendering only.
/// </summary>
[TemplatePart(Name = "PART_Surface", Type = typeof(Canvas))]
[TemplatePart(Name = "PART_Grid", Type = typeof(Canvas))]
[TemplatePart(Name = "PART_SelectionAdorner", Type = typeof(Canvas))]
public sealed class EditorCanvas : Control
{
    private Canvas? _surface;
    private Canvas? _grid;
    private Canvas? _selectionAdorner;
    private CanvasDocumentController? _document;
    private readonly CanvasViewportController _viewport = new();
    private CanvasPoint? _pointerOrigin;
    private CanvasPoint _lastPointer;
    private CanvasPoint _previewDelta;
    private uint _capturedPointer;
    private bool _dragging;
    private bool _panning;
    private bool _drawing;
    private bool _inking;
    private CanvasInkSession? _inkSession;

    public static readonly DependencyProperty ToolProperty = DependencyProperty.Register(nameof(Tool), typeof(EditorCanvasTool), typeof(EditorCanvas), new PropertyMetadata(EditorCanvasTool.Select));
    public static readonly DependencyProperty IsGridVisibleProperty = DependencyProperty.Register(nameof(IsGridVisible), typeof(bool), typeof(EditorCanvas), new PropertyMetadata(true, OnVisualPropertyChanged));
    public static readonly DependencyProperty SnapToGridProperty = DependencyProperty.Register(nameof(SnapToGrid), typeof(bool), typeof(EditorCanvas), new PropertyMetadata(false));
    public static readonly DependencyProperty GridSizeProperty = DependencyProperty.Register(nameof(GridSize), typeof(double), typeof(EditorCanvas), new PropertyMetadata(16d, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(EditorCanvas), new PropertyMetadata(false));

    public EditorCanvas()
    {
        DefaultStyleKey = typeof(EditorCanvas);
        _viewport.Changed += OnViewportChanged;
        KeyDown += OnKeyDown;
        SizeChanged += (_, _) => Render();
    }

    public EditorCanvasTool Tool { get => (EditorCanvasTool)GetValue(ToolProperty); set => SetValue(ToolProperty, value); }
    public bool IsGridVisible { get => (bool)GetValue(IsGridVisibleProperty); set => SetValue(IsGridVisibleProperty, value); }
    public bool SnapToGrid { get => (bool)GetValue(SnapToGridProperty); set => SetValue(SnapToGridProperty, value); }
    public double GridSize { get => (double)GetValue(GridSizeProperty); set => SetValue(GridSizeProperty, Math.Clamp(value, 4, 256)); }
    public bool IsReadOnly { get => (bool)GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    public CanvasDocumentController? Document
    {
        get => _document;
        set
        {
            if (ReferenceEquals(_document, value)) return;
            if (_document is not null) _document.Changed -= OnDocumentChanged;
            _document = value;
            if (_document is not null) _document.Changed += OnDocumentChanged;
            Render();
        }
    }
    public CanvasViewportController Viewport => _viewport;
    public CanvasInkSession? InkSession
    {
        get => _inkSession;
        set
        {
            if (ReferenceEquals(_inkSession, value)) return;
            if (_inkSession is not null) _inkSession.Changed -= OnInkChanged;
            _inkSession = value;
            if (_inkSession is not null) _inkSession.Changed += OnInkChanged;
            Render();
        }
    }
    public CanvasInkStyle InkStyle { get; set; } = new();
    public event EventHandler<EditorCanvasObjectInvokedEventArgs>? ObjectInvoked;
    public event EventHandler<EditorCanvasSelectionChangedEventArgs>? SelectionChanged;
    public event EventHandler? ViewportChanged;

    protected override void OnApplyTemplate()
    {
        DetachSurfaceHandlers();
        base.OnApplyTemplate();
        _surface = GetTemplateChild("PART_Surface") as Canvas;
        _grid = GetTemplateChild("PART_Grid") as Canvas;
        _selectionAdorner = GetTemplateChild("PART_SelectionAdorner") as Canvas;
        AttachSurfaceHandlers();
        Render();
    }

    public void FitToContent(CanvasSize? viewport = null)
    {
        var snapshot = _document?.Snapshot ?? CanvasDocumentSnapshot.Empty;
        if (snapshot.Objects.Length == 0) return;
        var bounds = snapshot.Objects.Select(item => item.Bounds).Aggregate(Union);
        var size = viewport ?? new CanvasSize(ActualWidth, ActualHeight);
        _viewport.Fit(bounds, size);
    }

    public void ClearSelection() => _document?.Select(Array.Empty<string>());

    private static CanvasRect Union(CanvasRect left, CanvasRect right)
    {
        var x = Math.Min(left.X, right.X);
        var y = Math.Min(left.Y, right.Y);
        return new CanvasRect(x, y, Math.Max(left.Right, right.Right) - x, Math.Max(left.Bottom, right.Bottom) - y);
    }

    private void AttachSurfaceHandlers()
    {
        if (_surface is null) return;
        _surface.PointerPressed += OnPointerPressed;
        _surface.PointerMoved += OnPointerMoved;
        _surface.PointerReleased += OnPointerReleased;
        _surface.PointerCanceled += OnPointerCanceled;
        _surface.PointerWheelChanged += OnPointerWheelChanged;
        _surface.DoubleTapped += OnDoubleTapped;
    }

    private void DetachSurfaceHandlers()
    {
        if (_surface is null) return;
        _surface.PointerPressed -= OnPointerPressed;
        _surface.PointerMoved -= OnPointerMoved;
        _surface.PointerReleased -= OnPointerReleased;
        _surface.PointerCanceled -= OnPointerCanceled;
        _surface.PointerWheelChanged -= OnPointerWheelChanged;
        _surface.DoubleTapped -= OnDoubleTapped;
    }

    private void OnDocumentChanged(object? sender, CanvasDocumentChangedEventArgs args)
    {
        if (!DispatcherQueue.HasThreadAccess)
        {
            DispatcherQueue.TryEnqueue(() => OnDocumentChanged(sender, args));
            return;
        }
        Render();
        if (!args.Previous.SelectedIds.SetEquals(args.Current.SelectedIds))
            SelectionChanged?.Invoke(this, new EditorCanvasSelectionChangedEventArgs(args.Current.SelectedIds, args.IsUserInitiated));
    }

    private void OnInkChanged(object? sender, CanvasInkChangedEventArgs args)
    {
        if (!DispatcherQueue.HasThreadAccess)
        {
            DispatcherQueue.TryEnqueue(() => OnInkChanged(sender, args));
            return;
        }
        Render();
    }

    private void OnViewportChanged(object? sender, EventArgs args) { Render(); ViewportChanged?.Invoke(this, EventArgs.Empty); }
    private static void OnVisualPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) => ((EditorCanvas)sender).Render();

    private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
    {
        if (_surface is null) return;
        var point = args.GetCurrentPoint(_surface);
        var screen = new CanvasPoint(point.Position.X, point.Position.Y);
        _lastPointer = screen;
        _pointerOrigin = screen;
        _capturedPointer = point.PointerId;
        if (Tool == EditorCanvasTool.Pan || point.Properties.IsMiddleButtonPressed)
        {
            _panning = true;
            _surface.CapturePointer(args.Pointer);
            args.Handled = true;
            return;
        }
        if (Tool == EditorCanvasTool.Ink && !IsReadOnly && InkSession is not null)
        {
            _inking = InkSession.Begin(CreatePressureSample(point, _viewport), InkStyle);
            if (_inking)
            {
                _surface.CapturePointer(args.Pointer);
                args.Handled = true;
            }
            return;
        }
        if ((Tool == EditorCanvasTool.Rectangle || Tool == EditorCanvasTool.Text) && !IsReadOnly)
        {
            _drawing = true;
            _surface.CapturePointer(args.Pointer);
            args.Handled = true;
            return;
        }
        if (Tool != EditorCanvasTool.Select || IsReadOnly) return;
        var hit = _document?.HitTest(_viewport.ScreenToWorld(screen));
        _document?.Select(hit is null ? Array.Empty<string>() : new[] { hit.Id });
        _dragging = hit is not null;
        if (_dragging) _surface.CapturePointer(args.Pointer);
        args.Handled = hit is not null;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
    {
        if (_surface is null || (!_dragging && !_panning && !_drawing) || args.Pointer.PointerId != _capturedPointer) return;
        var point = args.GetCurrentPoint(_surface).Position;
        var current = new CanvasPoint(point.X, point.Y);
        var delta = current - _lastPointer;
        if (_panning) _viewport.PanBy(delta);
        else if (_inking && InkSession is not null) InkSession.AddSample(CreatePressureSample(args.GetCurrentPoint(_surface), _viewport));
        else if (_dragging) { _previewDelta += new CanvasPoint(delta.X / Math.Max(.01, _viewport.Transform.Zoom), delta.Y / Math.Max(.01, _viewport.Transform.Zoom)); Render(); }
        _lastPointer = current;
        args.Handled = true;
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs args) => EndPointer(args.Pointer.PointerId);
    private void OnPointerCanceled(object sender, PointerRoutedEventArgs args) => EndPointer(args.Pointer.PointerId, true);
    private void EndPointer(uint pointerId, bool cancel = false)
    {
        if (_surface is null || pointerId != _capturedPointer) return;
        if (_inking && InkSession is not null)
        {
            var stroke = cancel ? null : InkSession.Complete();
            if (stroke is not null && _document is not null && stroke.Samples.Length > 1)
                _document.Insert(new CanvasObject(stroke.Id, stroke.Bounds, stroke));
            if (cancel) InkSession.Cancel();
        }
        if (_drawing && !cancel && _document is not null && _pointerOrigin is CanvasPoint origin)
        {
            var end = _viewport.ScreenToWorld(_lastPointer);
            var start = _viewport.ScreenToWorld(origin);
            var bounds = CanvasRect.FromPoints(start, end);
            if (bounds.Width >= 8 || bounds.Height >= 8)
            {
                if (Tool == EditorCanvasTool.Text && bounds.Width < 120) bounds = bounds with { Width = 120 };
                if (Tool == EditorCanvasTool.Text && bounds.Height < 44) bounds = bounds with { Height = 44 };
                _document.Insert(new CanvasObject($"object-{Guid.NewGuid():N}", bounds, Tool == EditorCanvasTool.Text ? "Text object" : "Rectangle"));
            }
        }
        if (_dragging && !cancel && _document is not null && (_previewDelta.X != 0 || _previewDelta.Y != 0))
        {
            var delta = SnapToGrid ? Snap(_previewDelta) : _previewDelta;
            _document.MoveSelection(delta);
        }
        _previewDelta = default;
        _dragging = false;
        _panning = false;
        _drawing = false;
        _inking = false;
        _surface.ReleasePointerCaptures();
        Render();
    }

    private CanvasPoint Snap(CanvasPoint point) => new(Math.Round(point.X / GridSize) * GridSize, Math.Round(point.Y / GridSize) * GridSize);
    private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs args)
    {
        if (_surface is null) return;
        var point = args.GetCurrentPoint(_surface);
        var factor = point.Properties.MouseWheelDelta > 0 ? 1.1 : 1 / 1.1;
        _viewport.ZoomAt(factor, new CanvasPoint(point.Position.X, point.Position.Y));
        args.Handled = true;
    }

    private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs args)
    {
        if (_surface is null) return;
        var p = args.GetPosition(_surface);
        var item = _document?.HitTest(_viewport.ScreenToWorld(new CanvasPoint(p.X, p.Y)));
        if (item is not null) ObjectInvoked?.Invoke(this, new EditorCanvasObjectInvokedEventArgs(item, _viewport.ScreenToWorld(new CanvasPoint(p.X, p.Y)), true));
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs args)
    {
        if (_document is null) return;
        if (args.Key == Windows.System.VirtualKey.Delete && !IsReadOnly) { _document.DeleteSelection(); args.Handled = true; return; }
        if (args.Key == Windows.System.VirtualKey.Escape) { if (_inking) InkSession?.Cancel(); _document.Select(Array.Empty<string>()); args.Handled = true; return; }
        var delta = args.Key switch { Windows.System.VirtualKey.Left => new CanvasPoint(-1, 0), Windows.System.VirtualKey.Right => new CanvasPoint(1, 0), Windows.System.VirtualKey.Up => new CanvasPoint(0, -1), Windows.System.VirtualKey.Down => new CanvasPoint(0, 1), _ => default };
        if ((delta.X != 0 || delta.Y != 0) && !IsReadOnly) { _document.MoveSelection(SnapToGrid ? Snap(delta) : delta); args.Handled = true; }
    }

    private void Render()
    {
        if (_surface is null) return;
        _surface.Children.Clear();
        if (_grid is not null)
        {
            _grid.Visibility = IsGridVisible ? Visibility.Visible : Visibility.Collapsed;
            _grid.Children.Clear();
            if (IsGridVisible) RenderGrid();
        }
        var snapshot = _document?.Snapshot ?? CanvasDocumentSnapshot.Empty;
        foreach (var item in snapshot.Objects.OrderBy(item => item.ZIndex))
        {
            if (item.Payload is CanvasStroke stroke)
            {
                RenderStroke(stroke, snapshot.SelectedIds.Contains(item.Id));
                continue;
            }
            var bounds = snapshot.SelectedIds.Contains(item.Id) && _previewDelta != default ? item.Translate(_previewDelta).Bounds : item.Bounds;
            var border = new Border { Width = Math.Max(1, bounds.Width * _viewport.Transform.Zoom), Height = Math.Max(1, bounds.Height * _viewport.Transform.Zoom), Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 55, 90, 130)), Opacity = item.IsLocked ? .42 : .86, CornerRadius = new CornerRadius(6), Tag = item.Id };
            if (snapshot.SelectedIds.Contains(item.Id))
            {
                border.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                border.BorderThickness = new Thickness(2);
            }
            if (item.Payload is string label)
            {
                border.Child = new TextBlock { Text = label, Margin = new Thickness(12), TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center };
            }
            AutomationProperties.SetName(border, $"Canvas object {item.Id}");
            var screen = _viewport.WorldToScreen(new CanvasPoint(bounds.X, bounds.Y));
            Canvas.SetLeft(border, screen.X); Canvas.SetTop(border, screen.Y); _surface.Children.Add(border);
        }
        if (_inking && InkSession?.CurrentStroke is { } activeStroke) RenderStroke(activeStroke, false);
    }

    private void RenderStroke(CanvasStroke stroke, bool selected)
    {
        if (_surface is null || stroke.Samples.IsDefaultOrEmpty) return;
        var polyline = new Polyline
        {
            Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb((byte)(selected ? 255 : 220), (byte)(stroke.Style.Color >> 16), (byte)(stroke.Style.Color >> 8), (byte)stroke.Style.Color)),
            StrokeThickness = Math.Max(1, stroke.Samples.Average(sample => stroke.Style.WidthAt(sample.Pressure)) * _viewport.Transform.Zoom),
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            IsHitTestVisible = false,
        };
        foreach (var sample in stroke.Samples)
        {
            var screen = _viewport.WorldToScreen(sample.Point);
            polyline.Points.Add(new Windows.Foundation.Point(screen.X, screen.Y));
        }
        _surface.Children.Add(polyline);
    }

    private static CanvasPressureSample CreatePressureSample(Microsoft.UI.Input.PointerPoint point, CanvasViewportController viewport)
    {
        var pressure = point.Properties.Pressure;
        var kind = point.PointerDevice.PointerDeviceType switch
        {
            Microsoft.UI.Input.PointerDeviceType.Pen => CanvasInputDeviceKind.Pen,
            Microsoft.UI.Input.PointerDeviceType.Touch => CanvasInputDeviceKind.Touch,
            Microsoft.UI.Input.PointerDeviceType.Mouse => CanvasInputDeviceKind.Mouse,
            _ => CanvasInputDeviceKind.Unknown,
        };
        return new CanvasPressureSample(viewport.ScreenToWorld(new CanvasPoint(point.Position.X, point.Position.Y)), pressure <= 0 && kind == CanvasInputDeviceKind.Mouse ? 1 : pressure, Timestamp: point.Timestamp, DeviceKind: kind);
    }

    private void RenderGrid()
    {
        if (_grid is null || GridSize <= 0 || ActualWidth <= 0 || ActualHeight <= 0) return;
        var step = GridSize * _viewport.Transform.Zoom;
        if (step < 8) return;
        Brush? brush = null;
        if (Application.Current.Resources.ContainsKey("SeniorCanvasGridBrush") && Application.Current.Resources["SeniorCanvasGridBrush"] is Brush resourceBrush) brush = resourceBrush;
        var startX = Mod(_viewport.Transform.Offset.X, step);
        var startY = Mod(_viewport.Transform.Offset.Y, step);
        var count = 0;
        for (var x = startX; x <= ActualWidth && count++ < 240; x += step)
            _grid.Children.Add(new Line { X1 = x, X2 = x, Y1 = 0, Y2 = ActualHeight, Stroke = brush, StrokeThickness = 1 });
        count = 0;
        for (var y = startY; y <= ActualHeight && count++ < 240; y += step)
            _grid.Children.Add(new Line { X1 = 0, X2 = ActualWidth, Y1 = y, Y2 = y, Stroke = brush, StrokeThickness = 1 });
    }

    private static double Mod(double value, double divisor)
    {
        var result = value % divisor;
        return result < 0 ? result + divisor : result;
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);
}
