# ImmersiveNowPlaying Integration

## Dependencies

media.media-player-chrome, media.timed-text-view

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Only composes MediaPlayerChrome/TimedTextView; host licenses art and supplies color extraction. Unload stops visual sampling; no DRM/decoding.
