# DetachablePlayerHost Specification

## Goal

Provide an engine-neutral Inline, Detaching, Detached, Attaching, and Failed state machine backed by host-owned floating requests.

## Non-goals

No `Window`/`AppWindow` creation, playback-session migration or duplication, file-path persistence, network access, or media commands.

## Public API

`PlayerId`, `PreferredSize` (default 480×270, clamped to 160–4096×90–4096), `OwnerClosePolicy`, `IsAlwaysOnTop`, `State`, `ActiveRequest`, `AttachHost`, `DetachHost`, `DetachAsync`, `AttachAsync`, and `ToggleAsync`. Operations return Success/Rejected/Cancelled/Failed with stable error codes.

## State and behavior

Detach starts from Inline/Failed; Attach starts from Detached. A single gate serializes operations; host replacement, owner close, unload, or cancellation invalidates late responses. Owner close immediately returns Inline and raises `HostClosed`.

## Host boundary

`IFloatingWidgetHost.OpenAsync` receives the host content reference and request. Creation, close, window migration, and lifetime remain host-owned. The control retains `Content` and does not assume the external window accepted it.
