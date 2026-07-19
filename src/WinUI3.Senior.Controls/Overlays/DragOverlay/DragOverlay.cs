using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace WinUI3.Senior.Controls;

[Flags]
public enum DragOverlayOperations
{
    None = 0,
    Copy = 1,
    Move = 2,
    Link = 4
}

public enum DragOverlayState
{
    Hidden,
    Allowed,
    Forbidden,
    Dropping
}

public sealed class DragOverlayDropRequestedEventArgs(object? source, Point position, DragOverlayOperations operations) : EventArgs
{
    public object? Source { get; } = source;
    public Point Position { get; } = position;
    public DragOverlayOperations Operations { get; } = operations;
    public bool Accepted { get; set; }
}

public sealed class DragOverlayStateChangedEventArgs(DragOverlayState previousState, DragOverlayState currentState) : EventArgs
{
    public DragOverlayState PreviousState { get; } = previousState;
    public DragOverlayState CurrentState { get; } = currentState;
}

/// <summary>
/// Host-owned drag feedback layer. It never owns a data transfer operation,
/// captures a pointer, creates a window, or performs hit testing.
/// </summary>
public sealed class DragOverlay : Control
{
    public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register(
        nameof(IsVisible), typeof(bool), typeof(DragOverlay), new PropertyMetadata(false, OnVisualPropertyChanged));
    public static readonly DependencyProperty DragSourceProperty = DependencyProperty.Register(
        nameof(DragSource), typeof(object), typeof(DragOverlay), new PropertyMetadata(null));
    public static readonly DependencyProperty PreviewProperty = DependencyProperty.Register(
        nameof(Preview), typeof(object), typeof(DragOverlay), new PropertyMetadata(null));
    public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
        nameof(Caption), typeof(string), typeof(DragOverlay), new PropertyMetadata(string.Empty, OnVisualPropertyChanged));
    public static readonly DependencyProperty AllowedOperationsProperty = DependencyProperty.Register(
        nameof(AllowedOperations), typeof(DragOverlayOperations), typeof(DragOverlay), new PropertyMetadata(DragOverlayOperations.None, OnOperationsChanged));
    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        nameof(Position), typeof(Point), typeof(DragOverlay), new PropertyMetadata(new Point(), OnVisualPropertyChanged));
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State), typeof(DragOverlayState), typeof(DragOverlay), new PropertyMetadata(DragOverlayState.Hidden, OnStateChanged));

    private UIElement? _previewPart;
    private TextBlock? _captionPart;
    private TextBlock? _badgePart;
    private bool _dropping;

    public DragOverlay()
    {
        DefaultStyleKey = typeof(DragOverlay);
        IsHitTestVisible = false;
        IsTabStop = false;
    }

    public bool IsVisible { get => (bool)GetValue(IsVisibleProperty); private set => SetValue(IsVisibleProperty, value); }
    public object? DragSource { get => GetValue(DragSourceProperty); private set => SetValue(DragSourceProperty, value); }
    public object? Preview { get => GetValue(PreviewProperty); private set => SetValue(PreviewProperty, value); }
    public string Caption { get => (string)GetValue(CaptionProperty); private set => SetValue(CaptionProperty, value ?? string.Empty); }
    public DragOverlayOperations AllowedOperations { get => (DragOverlayOperations)GetValue(AllowedOperationsProperty); private set => SetValue(AllowedOperationsProperty, value); }
    public Point Position { get => (Point)GetValue(PositionProperty); private set => SetValue(PositionProperty, value); }
    public DragOverlayState State { get => (DragOverlayState)GetValue(StateProperty); private set => SetValue(StateProperty, value); }

    public event EventHandler<DragOverlayStateChangedEventArgs>? StateChanged;
    public event EventHandler<DragOverlayDropRequestedEventArgs>? DropRequested;

    protected override AutomationPeer OnCreateAutomationPeer() => new DragOverlayAutomationPeer(this);

    public void Show(object? source, object? preview, string? caption, DragOverlayOperations operations, Point position)
    {
        _dropping = false;
        DragSource = source;
        Preview = preview;
        Caption = caption ?? string.Empty;
        AllowedOperations = operations;
        Position = position;
        IsVisible = true;
        UpdateVisualState();
    }

    public bool Update(Point position, DragOverlayOperations operations, string? caption = null, object? preview = null)
    {
        if (!IsVisible) return false;
        Position = position;
        AllowedOperations = operations;
        if (caption is not null) Caption = caption;
        if (preview is not null) Preview = preview;
        return true;
    }

    public bool BeginDrop()
    {
        if (!IsVisible || AllowedOperations == DragOverlayOperations.None || _dropping) return false;
        _dropping = true;
        State = DragOverlayState.Dropping;
        var args = new DragOverlayDropRequestedEventArgs(DragSource, Position, AllowedOperations);
        DropRequested?.Invoke(this, args);
        if (args.Accepted) Hide(); else { _dropping = false; UpdateVisualState(); }
        return args.Accepted;
    }

    public void Hide()
    {
        _dropping = false;
        State = DragOverlayState.Hidden;
        IsVisible = false;
        DragSource = null;
        Preview = null;
        Caption = string.Empty;
        AllowedOperations = DragOverlayOperations.None;
        Position = new Point();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _previewPart = GetTemplateChild("PART_Preview") as UIElement;
        _captionPart = GetTemplateChild("PART_Caption") as TextBlock;
        _badgePart = GetTemplateChild("PART_Badge") as TextBlock;
        UpdateVisualState();
    }

    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DragOverlay)d).UpdateVisualState();
    private static void OnOperationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var overlay = (DragOverlay)d;
        if (overlay.IsVisible && !overlay._dropping) overlay.State = overlay.AllowedOperations == DragOverlayOperations.None ? DragOverlayState.Forbidden : DragOverlayState.Allowed;
        overlay.UpdateVisualState();
    }
    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var overlay = (DragOverlay)d;
        if ((DragOverlayState)e.OldValue != (DragOverlayState)e.NewValue) overlay.StateChanged?.Invoke(overlay, new DragOverlayStateChangedEventArgs((DragOverlayState)e.OldValue, (DragOverlayState)e.NewValue));
        overlay.UpdateVisualState();
    }
    private void UpdateVisualState()
    {
        if (IsVisible && !_dropping) State = AllowedOperations == DragOverlayOperations.None ? DragOverlayState.Forbidden : DragOverlayState.Allowed;
        VisualStateManager.GoToState(this, IsVisible ? State.ToString() : DragOverlayState.Hidden.ToString(), true);
        if (_captionPart is not null) _captionPart.Text = Caption;
        if (_previewPart is FrameworkElement preview) { Canvas.SetLeft(preview, Position.X + 16); Canvas.SetTop(preview, Position.Y + 16); }
        if (_badgePart is not null) _badgePart.Text = State == DragOverlayState.Forbidden ? "Not allowed" : string.Empty;
    }
}

internal sealed class DragOverlayAutomationPeer(DragOverlay owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Custom;
    protected override string GetClassNameCore() => nameof(DragOverlay);
    protected override string GetNameCore() => Owner is DragOverlay overlay && overlay.IsVisible ? overlay.Caption : "Drag feedback";
}
