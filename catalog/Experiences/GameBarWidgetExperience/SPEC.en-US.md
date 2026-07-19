# GameBarWidgetExperience Specification

## Goal

Provide a host-neutral Game Bar style widget state machine and interaction-mode requests.

## Non-goals

No window creation, system click-through changes, global hotkey registration, game/process data access, or persisted position.

## Public API

`WidgetId`, `PreferredSize`, `RecoveryHotKey`, `IsAlwaysOnTop`, `State`, `InteractionMode`, `AttachHost`, `DetachHost`, `OpenAsync`, `CloseAsync`, `SetInteractionMode`, `Minimize`, and `Restore`. `InteractionModeRequested` requires host confirmation for ClickThrough.

## Behavior

Open requests presentation through `IFloatingWidgetHost`; close/owner close returns Closed. ClickThrough is rejected without a recovery hotkey or host `Handled=true`. Minimized retains content and Restore returns to the prior interaction mode.
