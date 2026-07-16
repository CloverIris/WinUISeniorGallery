# IconPicker Integration

## Dependencies

foundation.resource-catalog

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Provider is async/cancellable with UI commit; no arbitrary file scan. Unload releases thumbnails and host persists favorites.
