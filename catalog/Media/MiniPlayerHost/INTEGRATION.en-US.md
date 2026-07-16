# MiniPlayerHost Integration

## Dependencies and Global Contracts

Depend on media.media-player-chrome and applicable Theme, Motion, Input, Accessibility, Localization, Resources, MediaPlayback/Windowing contracts; never redefine shared keys.

## Host and Platform APIs

Pure XAML/Composition on one DispatcherQueue with no window/media capability; Window close cancels animation and returns content ownership.

## Lifecycle, Threading, and Cross-window Behavior

Manage two slots inside the current Window; create no AppWindow, request no CompactOverlay, and never reparent XAML across windows. CompactOverlayHost/DetachablePlayerHost owns windowing and MediaPlayerChrome/IMediaPlaybackSession owns playback. Visual work stays on owning DispatcherQueue; background results carry version/cancellation. Ignore late callbacks after destruction, Window.Closed, or unload.

## Degradation and Errors

Missing capability/part/data preserves an accessible minimum path and localized reason; never report false success or log media/window/input data.

## Resources

Candidate resources use MiniPlayerHost prefix; exact keys freeze before ready. Never hard-code color, DPI, or Window size.

