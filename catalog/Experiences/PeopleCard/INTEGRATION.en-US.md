# PeopleCard Integration

## Dependencies

controls.overlay-menu

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes OverlayMenu; host supplies authorized contact/avatar/commands. No address-book read, PII cache/telemetry; unload cancels.
