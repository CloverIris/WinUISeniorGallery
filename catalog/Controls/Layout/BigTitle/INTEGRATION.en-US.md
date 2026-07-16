# BigTitle Integration

## Dependencies

foundation.theme-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Bind `ScrollViewer.ViewChanged` on UI thread and detach on unload. Update Composition/layout at most once per frame; no external API or capability.
