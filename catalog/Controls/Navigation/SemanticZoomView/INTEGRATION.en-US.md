# SemanticZoomView Integration

## Dependencies

foundation.input-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Both views share host data; switch on UI thread and cancel mapping/animation on unload. No system zoom capability.
