# PivotView Integration

## Dependencies

foundation.input-system, foundation.motion-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Collections/properties are UI-thread only; unload releases gestures/unrealized pages. Adjacent-page policy must bound memory.
