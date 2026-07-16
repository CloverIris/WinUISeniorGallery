# PropertyGrid Integration

## Dependencies

foundation.accessibility-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Provider describes/reads/writes/validates with UI commit; unload ends edit/unbinds. No private reflection or value logging.
