# ColorPickerEx Integration

## Dependencies

foundation.accessibility-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
UI-thread commit; eyedropper is cancellable host provider handling capability; unload cancels. History is memory-only.
