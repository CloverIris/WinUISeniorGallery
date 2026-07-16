# SnackbarHost Specification

## Goals and non-goals

Maintain a strictly isolated, stable-priority, single-message queue per WinUI window with timeout, action, programmatic cancellation, CancellationToken, deduplication, and screen-reader announcements. System toasts, cross-process notification, history, and background activation are excluded.

## Public API

```csharp
public enum SnackbarPriority { Low, Normal, High, Critical }
public enum SnackbarVisualKind { Informational, Success, Warning, Error }
public enum SnackbarCompletionReason { TimedOut, ActionInvoked, Dismissed, Cancelled, HostDestroyed, Replaced }
public enum SnackbarPlacement { BottomLeft, BottomCenter, BottomRight, TopLeft, TopCenter, TopRight }
public sealed record SnackbarAction(string Label, ICommand Command, object? CommandParameter = null);
public sealed record SnackbarRequest(Guid Id, string Message, SnackbarVisualKind Kind = Informational,
    SnackbarPriority Priority = Normal, TimeSpan? Duration = null, SnackbarAction? Action = null,
    string? DeduplicationKey = null, bool IsDismissible = true, string? AutomationAnnouncement = null);
public sealed record SnackbarResult(Guid RequestId, SnackbarCompletionReason Reason);

public sealed class SnackbarHostRegistration : IDisposable
{
    public WindowId WindowId { get; }
    public string? HostName { get; }
    public bool IsValid { get; }
    public void Dispose();
}

public sealed class SnackbarHost : Control
{
    public string? HostName { get; set; }
    public SnackbarPlacement Placement { get; set; } // BottomCenter
    public bool IsPaused { get; set; }
    public event EventHandler<SnackbarShownEventArgs> Shown;
    public event EventHandler<SnackbarClosedEventArgs> Closed;
}
public interface ISnackbarService
{
    SnackbarHostRegistration Register(WindowId windowId, SnackbarHost host, string? hostName = null);
    Task<SnackbarResult> ShowAsync(SnackbarHostRegistration target, SnackbarRequest request, CancellationToken cancellationToken = default);
    bool Cancel(SnackbarHostRegistration target, Guid requestId);
}
```

`SnackbarHostRegistration` is an unforgeable disposable target token exposing `WindowId`, `HostName`, and `IsValid`. `ShowAsync` after disposal returns `HostDestroyed`. A Host can register once; a window can have differently named Hosts and `(WindowId, HostName)` is unique.

`Id` is non-empty and unique among unfinished requests in the target; `Message` and Action Label are non-empty. Explicit Duration is `[1s, 60s]`. Defaults are 5s for Low/Normal, 8s for High, and 10s for Critical. The global accessibility timeout contract may extend, capped at 60s.

## Queue, priority, and deduplication

One message is visible per Host. Higher priority never preempts the visible item. Pending order is Priority descending and FIFO within a priority; completion takes the earliest highest-priority item.

Null `DeduplicationKey` disables deduplication. For equal non-null keys:

1. A pending match is replaced in its queue slot; the old task completes `Replaced`.
2. A visible match completes `Replaced`, updates in place, and restarts timing without close/open animation.
3. Key comparison is Ordinal and scoped to one Host.

`Cancel` or CancellationToken removes pending work as `Cancelled`; visible work closes with animation then returns `Cancelled`. Completion races use the first atomic commit. Action first commits `ActionInvoked`, then executes its command on the UI thread. Command exceptions follow DispatcherQueue unhandled-exception reporting without changing completion.

## State, template, and behavior

Host states: `Empty`, `Showing`, `Visible`, `Closing`, `Destroyed`. Timeout remaining freezes while the window is inactive, pointer is over, keyboard focus is inside, or `IsPaused=true`, then resumes. Window destruction skips animation and completes all current/pending work as `HostDestroyed`, never forwarding it.

Parts: `PART_Root`, `PART_Surface`, `PART_MessagePresenter`, optional `PART_IconPresenter`, `PART_ActionButton`, `PART_DismissButton`, `PART_LiveRegion`. Visual groups: `DisplayStates`, `KindStates`, `InteractionStates`, `PlacementStates`. Missing Surface or MessagePresenter raises a template exception; optional parts hide safely.

Escape dismisses only when focus is inside and `IsDismissible`; the Dismiss button follows the same rule. Action runs once. An empty Host is not hit-testable.
