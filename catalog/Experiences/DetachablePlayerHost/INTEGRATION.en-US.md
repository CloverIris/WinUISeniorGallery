# DetachablePlayerHost Integration

## Dependencies

media.media-player-chrome, windowing.compact-overlay-host

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes media Chrome and CompactOverlayHost; Windowing owns AppWindow, Media owns Session, experience orchestrates. Failure rolls back Inline; unload detaches without default stop.
