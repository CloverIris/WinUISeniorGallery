# ContentRail Integration

## Dependencies

foundation.input-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Collections update on UI thread; unload clears containers/handlers. No capability; 1,000 items realize viewport buffer only with at most one visual update/frame.
