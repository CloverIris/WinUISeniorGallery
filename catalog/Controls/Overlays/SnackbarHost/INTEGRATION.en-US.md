# SnackbarHost Integration

## Registration and lifecycle

Each window places a Host as the last child of a full-client root Grid and registers it with the real `WindowId`. DI may make `ISnackbarService` process-singleton, but every Show carries a `SnackbarHostRegistration`; implicit “most recent active window” resolution is forbidden.

The service serializes enqueue, timing, events, and commands through each Host's `DispatcherQueue`. Background threads may call `ShowAsync`, but Request is an immutable snapshot. Results may be awaited in any context. Host/window destruction or Registration disposal completes all tasks and detaches handlers.

## Contracts and platform

Consumes Theme, Motion, and Accessibility contracts. `WindowId` is `Microsoft.UI.WindowId`, with window lifecycle coordinated through `Microsoft.UI.Windowing`. No network, file, capability, or system-toast support is required. Resources register in the Controls theme dictionary; apps may override style but not queue semantics.

Disabled system animation shows/closes immediately; unavailable Composition falls back to XAML Opacity. Missing UIA live-region support still shows visual content and emits diagnostics. App suspension behaves as deactivation and freezes remaining time.

## Diagnostics

Debug logging records only Request Id, HostName, state, and completion reason by default, never Message or Action parameters. Duplicate IDs, invalid targets, and invalid parameters synchronously throw `ArgumentException`. An already-cancelled token returns a cancelled Task without enqueueing.
