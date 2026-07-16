# HomeScreen Integration

## Dependencies

controls.adaptive-grid, controls.widget-card

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Host supplies cancellable section provider/navigation; page does no network/cache. Composes only stable grid/card, cancels refresh on unload, preserves serializable scroll anchor.
