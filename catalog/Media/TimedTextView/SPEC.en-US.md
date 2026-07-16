# TimedTextView Specification

## Goals

- Represent file captions, lyrics, and live ASR with a stable Provider-independent model.
- Provide single-line, scrolling, word-highlight, and bilingual presentation in one control.
- Virtualize large documents and preserve reading position and assistive-technology stability during live revisions.

## Non-goals

- Do not recognize speech, call translation APIs, or store credentials.
- Do not parse SRT, VTT, LRC, or TTML; parsers are separate services.
- Do not control a player or change playback position; text clicks only raise request events.

## Public API

```csharp
public sealed class TimedTextView : Control
{
    public TimedTextDocument? Document { get; set; }
    public string? ActiveTrackId { get; set; }
    public TimeSpan Position { get; set; }
    public TimeSpan TimingOffset { get; set; }
    public TimedTextDisplayMode DisplayMode { get; set; }
    public bool IsAutoScrollEnabled { get; set; }
    public int ContextLineCount { get; set; }
    public event EventHandler<TimedTextActiveSegmentChangedEventArgs>? ActiveSegmentChanged;
    public event EventHandler<TimedTextSegmentInvokedEventArgs>? SegmentInvoked;
}

public enum TimedTextDisplayMode { SingleLine, ScrollingLyrics, Karaoke, Bilingual }
public enum TimedTextRevisionState { Interim, Final }
public enum TimedTextTrackRole { Captions, Subtitles, Lyrics, Translation, Transcript }

public sealed record TimedTextDocument(
    string Id, long Revision, IReadOnlyList<TimedTextTrack> Tracks);
public sealed record TimedTextTrack(
    string Id, string Language, TimedTextTrackRole Role,
    IReadOnlyList<TimedTextSegment> Segments);
public sealed record TimedTextSegment(
    string Id, TimeSpan Start, TimeSpan End, string Text,
    string? TranslatedText, TimedTextRevisionState RevisionState,
    IReadOnlyList<TimedTextWord> Words);
public sealed record TimedTextWord(
    string Id, TimeSpan Start, TimeSpan End, string Text,
    string? TranslatedText, TimedTextRevisionState RevisionState);
```

Defaults are `DisplayMode=SingleLine`, `TimingOffset=0`, `IsAutoScrollEnabled=true`, and `ContextLineCount=2`. Effective query time is `Position + TimingOffset`. `ContextLineCount` is coerced to 0–10.

Event arguments include document, track, Segment, query position, and user-initiated state. `SegmentInvoked` only expresses navigation intent; the host decides whether to Seek.

## Data and Revision Rules

Document, Track, Segment, and Word are immutable snapshots. `Revision` for the same Document `Id` increases monotonically; snapshots equal to or below the current Revision are ignored. A changed Document `Id` may restart Revision. Segment/Word `Id` stays stable within a Document. Interim may be replaced by the same ID in a later snapshot; Final also changes only through a higher Document Revision. Deletion means the new snapshot no longer contains that ID.

The control selects `ActiveTrackId`; when null or missing, choose the first track best matching the current UI language, otherwise the first track. Bilingual first uses `TranslatedText` on the same Segment, then a time-overlapping Translation-role track; if still absent, show only original text with no empty placeholder.

Invalid rules: ignore Segment/Word with `End <= Start`; clip Word to its parent Segment range; for empty or duplicate `Id`, retain the first item; stable-sort Segment by Start, End, and input order. Never mutate input collections.

## Active Text State

Time intervals are `[Start, End)`. When Segments overlap, select the latest Start, then the first in sorted order; other overlaps may remain visible in scrolling mode. During gaps, SingleLine/Karaoke are empty while ScrollingLyrics/Bilingual retain context without active highlight.

Karaoke selects the active Word inside the active Segment by the same rule. With no valid Words, fall back to whole-Segment highlight. `ActiveSegmentChanged` fires only when active ID changes; high-frequency position updates never repeat one ID.

Manual scrolling suspends auto-centering for five seconds; a resume action or activation of a new Segment far outside the viewport restores it. Live revisions never steal keyboard focus; same-ID height changes anchor the active item.

## Template Parts

The fixed content and interaction states are:

```text
Empty --> Indexing --> Ready
Ready --> ActiveSegmentChanged --> Ready
Ready --> UserScrolling --> AutoScrollSuspended --> Ready
Ready --> ApplyingHigherRevision --> Ready
Any --> Empty               (document removed or no valid track)
Any --> Unloaded            (cancel layout and late updates)
```

- `PART_RootGrid`: root layer.
- `PART_SingleLinePresenter`: single-line captions.
- `PART_ScrollViewer` and `PART_ItemsRepeater`: virtualized lyrics/transcript.
- `PART_KaraokePresenter`: word highlight.
- `PART_PrimaryTextPresenter`, `PART_TranslationTextPresenter`: bilingual layers.
- `PART_EmptyPresenter`, `PART_LiveRegion`: empty state and announcements.

Missing parts other than root degrade the affected mode to SingleLine. Visual state groups are `DisplayModeStates`, `ContentStates`, `RevisionStates`, `AutoScrollStates`, and `CommonStates`.

## Failure Modes

- Null document, no tracks, or entirely invalid content shows empty state and never throws.
- Stale Revision, post-unload Provider updates, and events from a switched document are ignored.
- Missing translation, incomplete Word timing, or unrealized virtualized items use deterministic degradation.
- The control does not log text content, Provider errors, or user media position.
