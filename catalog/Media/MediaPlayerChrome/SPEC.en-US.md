# MediaPlayerChrome Specification

## Goals

- Use one control layer with system and third-party playback engines.
- Present consistent, accessible operations for VOD, live, and Live DVR.
- Let the host explicitly handle full-screen and compact-overlay requests without implicit window manipulation.

## Non-goals

- Do not parse media, own sources, or select tracks.
- Do not create `AppWindow`, call `CompactOverlayPresenter`, or migrate XAML content.
- Do not implement playlists, casting, or ASR in P0.

## Public API

```csharp
public sealed class MediaPlayerChrome : Control
{
    public IMediaPlaybackSession? PlaybackSession { get; set; }
    public MediaChromeDisplayMode DisplayMode { get; set; }
    public bool IsAutoHideEnabled { get; set; }
    public TimeSpan AutoHideDelay { get; set; }
    public bool IsCompactOverlayAvailable { get; set; }
    public bool IsFullScreen { get; set; }
    public bool IsCompactOverlay { get; set; }
    public event EventHandler<MediaChromePresentationRequestedEventArgs>? PresentationRequested;
}

public enum MediaChromeDisplayMode { Full, Compact, Minimal }
public enum MediaChromePresentationMode { Inline, FullScreen, CompactOverlay }

public sealed class MediaChromePresentationRequestedEventArgs : EventArgs
{
    public MediaChromePresentationMode RequestedMode { get; }
    public bool Handled { get; set; }
}

public sealed class MediaPlayerElementPlaybackSessionAdapter : IMediaPlaybackSession, IDisposable
{
    public MediaPlayerElementPlaybackSessionAdapter(MediaPlayerElement element);
}
```

`IMediaPlaybackSession` is a shared contract and must expose read-only playback state, capabilities, position, time range, volume, and rate plus asynchronous `Play`, `Pause`, `Stop`, `Seek`, `SetVolume`, and `SetPlaybackRate` commands. State changes use detachable events or observable notifications. Every asynchronous command accepts `CancellationToken`.

Property rules: `PlaybackSession` may be null and then presents a disabled state. `AutoHideDelay` below one second is coerced to one second. `IsFullScreen` and `IsCompactOverlay` are host-confirmed facts; request events do not change them automatically.

## State Model

The control maps the session to `Unavailable`, `Loading`, `Ready`, `Playing`, `Paused`, `Buffering`, `Ended`, and `Failed`. Replacing a session immediately detaches the old session, cancels its commands, and refreshes all state. A running command disables only conflicting actions; failures restore operability and show non-blocking feedback.

Auto-hide applies only while `Playing` and when the pointer is outside, keyboard focus is absent, no menu is open, and timeline scrubbing is inactive. Touch, pointer movement, keyboard, or gamepad input reveals controls and resets the timer.

## Template Parts

The fixed state transitions are:

```text
Unavailable --> Loading --> Ready
Ready/Paused/Ended --> Playing
Playing --> Paused | Buffering | Ended
Buffering --> Playing | Paused | Failed
Any session state --> Failed | Unavailable

Presentation (orthogonal, host-confirmed):
Inline <--> FullScreen
Inline <--> CompactOverlay
```

- `PART_RootGrid`: root visual layer.
- `PART_Timeline`: `MediaTimeline`.
- `PART_PlayPauseButton`, `PART_StopButton`: transport commands.
- `PART_VolumeSlider`, `PART_MuteButton`: volume.
- `PART_PlaybackRateButton`: rate menu.
- `PART_FullScreenButton`, `PART_CompactOverlayButton`: request-only actions.
- `PART_BufferingIndicator`, `PART_ErrorPresenter`: feedback.
- `PART_MoreButton`: compact overflow entry.

Missing non-root parts degrade by hiding the related capability and never crash. Visual state groups are `PlaybackStates`, `DisplayModeStates`, `VisibilityStates`, `PresentationStates`, and `CommonStates`.

## Behavior and Failure Modes

- Rapid repeated operations serialize commands of the same kind; a later Seek may cancel a previous uncommitted Seek.
- Unsupported capabilities are hidden or disabled with Automation help text.
- Session exceptions are caught and mapped to localizable errors, never propagated on the UI thread.
- If the host does not handle `PresentationRequested`, state remains unchanged; the control does not infer a window result.
- The adapter reads `MediaPlayerElement` on the UI thread and stops forwarding events after disposal.
