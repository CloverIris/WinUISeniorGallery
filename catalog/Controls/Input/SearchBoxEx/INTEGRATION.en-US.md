# SearchBoxEx Integration

## Dependencies

foundation.accessibility-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Provider is async/cancellable and dispatches results; unload cancels. Host supplies history/trends; no telemetry/persistence.
