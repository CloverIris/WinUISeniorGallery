# DynamicTile Integration

## Dependencies

foundation.motion-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Hosts provide data on UI thread; no notification/background capability. Unload stops timers/releases templates; reload waits a full interval from current face.
