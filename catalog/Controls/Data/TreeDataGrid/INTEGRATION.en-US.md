# TreeDataGrid Integration

## Dependencies

foundation.accessibility-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Child load is cancellable/versioned; unload cancels/releases containers. UI collections, background provider; no edit persistence.
