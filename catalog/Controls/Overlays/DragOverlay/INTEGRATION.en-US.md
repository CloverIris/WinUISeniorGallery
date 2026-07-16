# DragOverlay Integration

## Dependencies

foundation.input-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
UI thread, coalesce pointer updates once/frame; unload/deactivate forces Hide and releases DataPackage. No cross-process read.
