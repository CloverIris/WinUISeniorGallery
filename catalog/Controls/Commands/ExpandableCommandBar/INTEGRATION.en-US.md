# ExpandableCommandBar Integration

## Dependencies

foundation.input-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Commands are UI-thread only; unload releases capture/animation. No system edge gesture registration.
