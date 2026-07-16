# MediaTimeline Specification

## Goals

- Express fixed VOD ranges and continuously moving Live DVR windows through one API.
- Explicitly separate low-cost scrub preview from expensive player Seek.
- Clearly represent buffering, chapters, markers, and non-seekable areas on one track.

## Non-goals

- Do not call a player directly or guarantee media-position update frequency.
- Do not download, decode, or cache hover thumbnails.
- Do not edit chapters or markers or serve as a media-editing timeline.

## Public API

```csharp
public sealed class MediaTimeline : Control
{
    public MediaTimelineMode Mode { get; set; }
    public TimeSpan Minimum { get; set; }
    public TimeSpan Maximum { get; set; }
    public TimeSpan Position { get; set; }
    public DateTimeOffset? LiveWindowEndTime { get; set; }
    public TimeSpan LiveEdgeTolerance { get; set; }
    public double PlaybackRate { get; set; }
    public IReadOnlyList<MediaTimeRange> BufferedRanges { get; set; }
    public IReadOnlyList<MediaTimeRange> DisabledRanges { get; set; }
    public IReadOnlyList<MediaTimelineMarker> Chapters { get; set; }
    public IReadOnlyList<MediaTimelineMarker> Markers { get; set; }
    public bool IsSeekEnabled { get; set; }
    public TimeSpan KeyboardStep { get; set; }
    public TimeSpan LargeKeyboardStep { get; set; }
    public TimeSpan PreviewThrottleInterval { get; set; }
    public event EventHandler<MediaTimelinePreviewEventArgs>? PreviewPositionChanged;
    public event EventHandler<MediaTimelineSeekRequestedEventArgs>? SeekRequested;
    public event EventHandler? LiveEdgeRequested;
}

public enum MediaTimelineMode { VideoOnDemand, Live, LiveDvr }
public readonly record struct MediaTimeRange(TimeSpan Start, TimeSpan End);
public sealed record MediaTimelineMarker(string Id, TimeSpan Position, string? Label, object? Data);
```

Defaults are `Mode=VideoOnDemand`, `Minimum=0`, `LiveEdgeTolerance=3s`, `PlaybackRate=1.0`, `KeyboardStep=5s`, `LargeKeyboardStep=30s`, and `PreviewThrottleInterval=100ms`. `LiveWindowEndTime` only formats relative media time as wall-clock time and never participates in Seek calculations. `PlaybackRate` is presentation-only for a rate label and Automation value text; it never scales media time, stepping, or Seek. Non-finite or non-positive values present as 1.0.

Event arguments contain requested position, user-initiated state, and input kind. `SeekRequested` does not carry a `CancellationToken`: it is raised synchronously, the host performs asynchronous work, and reports the result through `Position`.

## Time Domain and Normalization

All positions use one `TimeSpan` media domain with closed interval `[Minimum, Maximum]`. `Maximum < Minimum` presents Unavailable. Ranges are clipped to the available interval; ranges with `End <= Start` are ignored; buffered and disabled ranges are independently sorted and merged. Chapters and markers are stable-sorted by position. Out-of-range items are not rendered and the input collection is not mutated.

A scrub point inside a disabled range is corrected to the nearest boundary; ties follow drag direction. Keyboard forward/backward skips the entire disabled range. If the host writes a disabled Position, only presentation is clamped and no reverse Seek is raised.

## State Model

States are `Unavailable`, `ReadOnly`, `Idle`, `PointerOver`, `Scrubbing`, and `AtLiveEdge`. `Live` is non-seekable and shows only current-live state and badge. In `LiveDvr`, `Maximum` is the moving Live Edge; `Position >= Maximum - LiveEdgeTolerance` enters `AtLiveEdge`.

Scrub start freezes one input-range snapshot. During scrub, `PreviewPositionChanged` is emitted no more often than `PreviewThrottleInterval`, without losing the final preview. Release raises exactly one `SeekRequested`. Cancellation raises no Seek and restores `Position`. Arrow, PageUp/PageDown, Home, and End each immediately raise one Seek. Live DVR End raises `LiveEdgeRequested`; if unhandled, request the current `Maximum`.

## Template Parts

The fixed state transitions are:

```text
Unavailable --> ReadOnly | Idle
Idle --> PointerOver --> Scrubbing
Scrubbing --> Idle          (commit or cancel)
Idle/PointerOver --> AtLiveEdge
AtLiveEdge --> Idle         (position leaves tolerance)
Any --> Unavailable         (invalid range or unload)
```

- `PART_RootGrid`: root layout.
- `PART_Track`: available-range track.
- `PART_BufferedRangesPresenter`, `PART_DisabledRangesPresenter`: range layers.
- `PART_ChapterPresenter`, `PART_MarkerPresenter`: marker layers.
- `PART_Progress`, `PART_Thumb`: position and scrub.
- `PART_ToolTip`, `PART_TimeText`, `PART_PlaybackRateText`, `PART_LiveBadge`, `PART_GoLiveButton`: feedback, rate, and live action.

Missing parts other than `PART_RootGrid` disable only their visual. Visual state groups are `CommonStates`, `InteractionStates`, `TimelineModeStates`, and `LiveStates`.

## Failure Modes

- Empty, concurrently replaced, or invalid collections do not crash; consume the latest complete snapshot.
- Unload cancels pending preview and raises no Seek afterward.
- If a moving window passes the scrub point, the final point is reclamped to the latest valid range at release.
- If the host rejects Seek, it leaves or writes the original `Position`; the control never assumes success.
