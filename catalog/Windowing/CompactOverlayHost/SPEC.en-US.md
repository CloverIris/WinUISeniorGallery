# CompactOverlayHost Specification

## Goal

Reliably switch one AppWindow between Inline and CompactOverlay presenters while preserving/restoring size and host-confirmed state.

## Host and Window Boundary

Create no second window, migrate no XAML content, and control no playback; host supplies existing AppWindow, handles close, and authorizes requests.

## Candidate Surface and Lock Conditions

Candidate concepts: AppWindow, RequestedMode, ConfirmedMode, PreferredOverlaySize, Enter/ExitRequested, TransitionFailed. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Inline --> EnteringOverlay --> Overlay\nOverlay --> ExitingOverlay --> Inline\nTransition --> Failed --> previous confirmed mode\nAny --> Closing --> Closed
```

## Candidate Template Parts and Visual Tree

Candidate is nonvisual; optional OverlayChromePresenter shows exit, topmost explanation, and minimum controls only. Never duplicate page content.

## Behavior and Failure Modes

Coalesce same requests and serialize opposites; update ConfirmedMode only after SetPresenter and state confirmation, restoring original size/presenter on failure.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

