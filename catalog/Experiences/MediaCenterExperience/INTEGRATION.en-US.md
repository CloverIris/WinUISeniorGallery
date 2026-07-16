# MediaCenterExperience Integration

## Dependencies

media.media-center-grid

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes media-center-grid; host provides catalog/play routes. No library scan/remote driver. Unload cancels paging.
