# MediaPlayback Integration

## Contract and dependencies

Code lives in `WinUI3.Senior.Core`; Media, Gallery, and tests reference only `IMediaPlaybackSession`. `MediaPlayerElementPlaybackSessionAdapter` is owned by MediaPlayerChrome and projects into this contract instead of pulling XAML types back into Core.

## Threading and lifetime

Providers read and modify UI/engine objects on their owning UI Dispatcher; background native callbacks check generation before ordered snapshot publication. Replacement, unload, or window close cancels commands, detaches subscriptions, and ignores late events.

## Privacy and diagnostics

The contract accepts no media source, path, or user text. Stable error codes may reach local diagnostics, while correlation IDs contain no user content; network, telemetry, and persistence are outside P0.
