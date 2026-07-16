# QuickResumeExperience Integration

## Dependencies

controls.content-rail

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes ContentRail; host creates/encrypts/expires/deletes snapshots and restores. Experience never captures/persists token; unload cancels.
