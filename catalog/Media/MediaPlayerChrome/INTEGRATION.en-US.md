# MediaPlayerChrome Integration

## Global Contracts

- Reference `IMediaPlaybackSession`, capabilities, and state types from `contracts/MediaPlayback`.
- Embed `MediaTimeline`, binding position and range in the same session time domain.
- Use global Theme, Motion, Input, Accessibility, Localization, and Resources contracts.

## Services and Platform APIs

The host owns Window, AppWindow, page content, media source, and `IMediaPlaybackSession` lifetime. The control neither creates/closes windows nor moves `MediaPlayerElement` or any XAML element to another XamlRoot. It only raises `PresentationRequested` and waits for host-confirmed state. On Window close, the host cancels requests before releasing session and adapter.

`MediaPlayerElementPlaybackSessionAdapter` is optional and wraps `Microsoft.UI.Xaml.Controls.MediaPlayerElement` plus its underlying `Windows.Media.Playback.MediaPlayer`. The host still owns sources, `SystemMediaTransportControls`, audio focus, and app lifecycle.

Full-screen and compact-overlay buttons only raise `PresentationRequested`. The host uses the Windowing module or its own `AppWindow` implementation and writes confirmed state to `IsFullScreen`/`IsCompactOverlay`. P0 has no package dependency on `WinUI3.Senior.Windowing`.

## Threading and Lifecycle

Setters and events are marshalled and coalesced on the control DispatcherQueue. High-frequency engine position notifications are throttled to at most 10 UI updates per second. Unload or session replacement detaches events, stops timers, and cancels commands; reload restores from the current snapshot.

## Permissions and Degradation

The control declares no network, microphone, or video-library capability. Hide compact overlay when unavailable; remain Inline when full screen is unhandled; lack of system transport controls does not affect local UI; an inaccessible disposed player makes the adapter `Unavailable`.

## Resources and Localization

All visible text, Automation names, errors, and time formats come from resources. Time follows the current locale and shortcut glyphs may vary by input device. Do not package third-party media assets.
