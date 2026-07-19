using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace WinUI3.Senior.Windowing;

/// <summary>Lifecycle of a host-owned window chrome attachment.</summary>
public enum WindowChromeState
{
    Detached,
    Attaching,
    Active,
    Reconfiguring,
    Failed,
    Closing,
    Closed
}

public enum WindowBackdropKind
{
    None,
    Mica,
    MicaAlt,
    Acrylic,
    HostProvided
}

public enum WindowingOperationStatus
{
    Success,
    Rejected,
    Cancelled,
    Failed
}

public sealed record WindowingOperationResult(
    WindowingOperationStatus Status,
    string? ErrorCode = null,
    string? Message = null)
{
    public bool IsSuccess => Status == WindowingOperationStatus.Success;

    public static WindowingOperationResult Succeeded() => new(WindowingOperationStatus.Success);
    public static WindowingOperationResult Rejected(string code, string? message = null) => new(WindowingOperationStatus.Rejected, code, message);
    public static WindowingOperationResult Cancelled() => new(WindowingOperationStatus.Cancelled, "cancelled");
    public static WindowingOperationResult Failed(string code, string? message = null) => new(WindowingOperationStatus.Failed, code, message);
}

public sealed record WindowChromeConfiguration(
    WindowBackdropKind Backdrop,
    bool ExtendIntoTitleBar,
    bool IsDragRegionEnabled);

/// <summary>
/// Adapter implemented by the application. The Windowing package never creates a Window or
/// calls AppWindow directly; all native operations remain host-owned.
/// </summary>
public interface IWindowChromeHost
{
    nint WindowHandle { get; }
    event EventHandler? Closed;
    Task<WindowingOperationResult> ConfigureChromeAsync(WindowChromeConfiguration configuration, CancellationToken cancellationToken = default);
}

public sealed record TitleBarInteractiveRegion(Rect Bounds, bool IsSystemButton = false, string? AutomationId = null);

public sealed class TitleBarRegionsChangedEventArgs : EventArgs
{
    public TitleBarRegionsChangedEventArgs(IReadOnlyList<TitleBarInteractiveRegion> regions) => Regions = regions;
    public IReadOnlyList<TitleBarInteractiveRegion> Regions { get; }
}

public enum FloatingWidgetState
{
    Closed,
    Creating,
    Visible,
    Minimized,
    Hidden,
    Closing,
    Failed
}

public enum FloatingWidgetOwnerClosePolicy
{
    Close,
    KeepAlive,
    RequestTransfer
}

public sealed record FloatingWidgetRequest(
    string WidgetId,
    Size PreferredSize,
    Point? PreferredPosition = null,
    bool IsAlwaysOnTop = true,
    FloatingWidgetOwnerClosePolicy OwnerClosePolicy = FloatingWidgetOwnerClosePolicy.Close);

public sealed record FloatingWidgetOperationResult(
    WindowingOperationStatus Status,
    string? ErrorCode = null,
    string? Message = null)
{
    public bool IsSuccess => Status == WindowingOperationStatus.Success;
    public static FloatingWidgetOperationResult Succeeded() => new(WindowingOperationStatus.Success);
    public static FloatingWidgetOperationResult Rejected(string code, string? message = null) => new(WindowingOperationStatus.Rejected, code, message);
    public static FloatingWidgetOperationResult Cancelled() => new(WindowingOperationStatus.Cancelled, "cancelled");
    public static FloatingWidgetOperationResult Failed(string code, string? message = null) => new(WindowingOperationStatus.Failed, code, message);
}

public interface IFloatingWidgetHost
{
    Task<FloatingWidgetOperationResult> OpenAsync(FloatingWidgetRequest request, object? content, CancellationToken cancellationToken = default);
    Task<FloatingWidgetOperationResult> CloseAsync(string widgetId, CancellationToken cancellationToken = default);
    Task<FloatingWidgetOperationResult> RestoreAsync(string widgetId, CancellationToken cancellationToken = default);
    Task<FloatingWidgetOperationResult> MinimizeAsync(string widgetId, CancellationToken cancellationToken = default);
    event EventHandler? OwnerClosed;
}

public enum CompactOverlayMode
{
    Inline,
    EnteringOverlay,
    Overlay,
    ExitingOverlay
}

public sealed record CompactOverlayRequest(CompactOverlayMode RequestedMode, Size PreferredSize);

public sealed record CompactOverlayOperationResult(
    WindowingOperationStatus Status,
    CompactOverlayMode ConfirmedMode,
    string? ErrorCode = null,
    string? Message = null)
{
    public bool IsSuccess => Status == WindowingOperationStatus.Success;
    public static CompactOverlayOperationResult Succeeded(CompactOverlayMode mode) => new(WindowingOperationStatus.Success, mode);
    public static CompactOverlayOperationResult Rejected(CompactOverlayMode mode, string code, string? message = null) => new(WindowingOperationStatus.Rejected, mode, code, message);
    public static CompactOverlayOperationResult Cancelled(CompactOverlayMode mode) => new(WindowingOperationStatus.Cancelled, mode, "cancelled");
    public static CompactOverlayOperationResult Failed(CompactOverlayMode mode, string code, string? message = null) => new(WindowingOperationStatus.Failed, mode, code, message);
}

public interface ICompactOverlayHost
{
    Task<CompactOverlayOperationResult> RequestModeAsync(CompactOverlayRequest request, CancellationToken cancellationToken = default);
}

public enum EdgeCommandPanelEdge
{
    Left,
    Right,
    Top,
    Bottom
}

/// <summary>Visual/interaction lifecycle of an edge command surface.</summary>
public enum EdgeCommandPanelState
{
    Closed,
    Opening,
    Open,
    Dragging,
    Closing
}

/// <summary>Reason a host requested that an edge panel close.</summary>
public enum EdgeCommandPanelDismissReason
{
    Programmatic,
    Escape,
    LightDismiss,
    DraggedPastThreshold,
    HostDeactivated
}

public sealed class EdgeCommandPanelDismissedEventArgs : EventArgs
{
    public EdgeCommandPanelDismissedEventArgs(EdgeCommandPanelDismissReason reason) => Reason = reason;
    public EdgeCommandPanelDismissReason Reason { get; }
}

public sealed record DockLayoutCommit(string LayoutId, DockLayoutKind Layout, string ZoneId);

public sealed class DockLayoutCommitRequestedEventArgs : EventArgs
{
    public DockLayoutCommitRequestedEventArgs(DockLayoutCommit commit) => Commit = commit;
    public DockLayoutCommit Commit { get; }
}

public sealed class DockLayoutFocusChangedEventArgs : EventArgs
{
    public DockLayoutFocusChangedEventArgs(string? previousZoneId, string? currentZoneId)
        => (PreviousZoneId, CurrentZoneId) = (previousZoneId, currentZoneId);

    public string? PreviousZoneId { get; }
    public string? CurrentZoneId { get; }
}

public enum TabTearOutState
{
    Idle,
    Pressed,
    Dragging,
    Requested,
    Cancelled
}

public sealed class TabTearOutDragEventArgs : EventArgs
{
    public TabTearOutDragEventArgs(string tabId, Point pointerPosition, double distance)
        => (TabId, PointerPosition, Distance) = (tabId, pointerPosition, distance);

    public string TabId { get; }
    public Point PointerPosition { get; }
    public double Distance { get; }
}

public enum ConnectedTransitionState
{
    Idle,
    Running,
    Completed,
    Cancelled
}

public sealed class ConnectedTransitionEventArgs : EventArgs
{
    public ConnectedTransitionEventArgs(string key, long sequence, double progress)
        => (Key, Sequence, Progress) = (key, sequence, Math.Clamp(progress, 0, 1));

    public string Key { get; }
    public long Sequence { get; }
    public double Progress { get; }
}
