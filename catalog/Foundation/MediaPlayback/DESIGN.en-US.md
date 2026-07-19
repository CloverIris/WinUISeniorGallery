# MediaPlayback Design

## Information hierarchy

This work item has no visual tree. A snapshot starts with session identity, state, and capabilities, then exposes time, audio parameters, and errors; controls choose visible or disabled actions from capabilities.

## Interaction and accessibility

The contract owns no input or focus. Every consumer supplies keyboard, screen-reader, theme, RTL, and Reduced Motion behavior from snapshots; error codes are never user-facing text.

## Modernization trade-offs

State is not represented by a static global player or mutable singleton. A window owns its session and Dispatcher, allowing `MediaPlayerElement`, C++, or a test fake while preventing cross-window XAML leakage.
