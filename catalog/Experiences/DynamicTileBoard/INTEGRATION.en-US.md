# DynamicTileBoard Integration

## Dependencies

controls.dynamic-tile

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Only composes DynamicTile; host owns data/background/persistence. No system tile/notification; unload stops visible updates.
