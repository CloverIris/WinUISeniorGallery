using System.Collections.Immutable;

namespace WinUI3.Senior.Media;

/// <summary>Specifies the presentation selected by <see cref="TimedTextView"/>.</summary>
public enum TimedTextDisplayMode
{
    SingleLine,
    ScrollingLyrics,
    Karaoke,
    Bilingual,
}

/// <summary>Identifies whether timed text is provisional or finalized by its source.</summary>
public enum TimedTextRevisionState
{
    Interim,
    Final,
}

/// <summary>Identifies the semantic use of a timed-text track.</summary>
public enum TimedTextTrackRole
{
    Captions,
    Subtitles,
    Lyrics,
    Translation,
    Transcript,
}

/// <summary>An immutable, revisioned timed-text document.</summary>
public sealed record TimedTextDocument(string Id, long Revision, IReadOnlyList<TimedTextTrack> Tracks)
{
    /// <summary>Creates a document with an empty, immutable track collection.</summary>
    public TimedTextDocument(string id, long revision)
        : this(id, revision, ImmutableArray<TimedTextTrack>.Empty)
    {
    }
}

/// <summary>An immutable language and semantic-purpose track.</summary>
public sealed record TimedTextTrack(
    string Id,
    string Language,
    TimedTextTrackRole Role,
    IReadOnlyList<TimedTextSegment> Segments);

/// <summary>An immutable timed text segment.</summary>
public sealed record TimedTextSegment(
    string Id,
    TimeSpan Start,
    TimeSpan End,
    string Text,
    string? TranslatedText,
    TimedTextRevisionState RevisionState,
    IReadOnlyList<TimedTextWord> Words);

/// <summary>An immutable word-level timing entry.</summary>
public sealed record TimedTextWord(
    string Id,
    TimeSpan Start,
    TimeSpan End,
    string Text,
    string? TranslatedText,
    TimedTextRevisionState RevisionState);

/// <summary>Contains an active-segment transition published by <see cref="TimedTextView"/>.</summary>
public sealed class TimedTextActiveSegmentChangedEventArgs : EventArgs
{
    public TimedTextActiveSegmentChangedEventArgs(
        TimedTextDocument document,
        TimedTextTrack track,
        TimedTextSegment? segment,
        TimeSpan position,
        bool isUserInitiated)
    {
        Document = document;
        Track = track;
        Segment = segment;
        Position = position;
        IsUserInitiated = isUserInitiated;
    }

    public TimedTextDocument Document { get; }
    public TimedTextTrack Track { get; }
    public TimedTextSegment? Segment { get; }
    public TimeSpan Position { get; }
    public bool IsUserInitiated { get; }
}

/// <summary>Contains a user request to navigate to a timed-text segment.</summary>
public sealed class TimedTextSegmentInvokedEventArgs : EventArgs
{
    public TimedTextSegmentInvokedEventArgs(
        TimedTextDocument document,
        TimedTextTrack track,
        TimedTextSegment segment,
        TimeSpan position,
        bool isUserInitiated)
    {
        Document = document;
        Track = track;
        Segment = segment;
        Position = position;
        IsUserInitiated = isUserInitiated;
    }

    public TimedTextDocument Document { get; }
    public TimedTextTrack Track { get; }
    public TimedTextSegment Segment { get; }
    public TimeSpan Position { get; }
    public bool IsUserInitiated { get; }
}

/// <summary>Reports the effective track selected after explicit or locale fallback.</summary>
public sealed class TimedTextTrackChangedEventArgs(TimedTextTrack? previous, TimedTextTrack? current) : EventArgs
{
    public TimedTextTrack? Previous { get; } = previous;
    public TimedTextTrack? Current { get; } = current;
}
