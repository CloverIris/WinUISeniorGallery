using System.Collections.Immutable;

namespace WinUI3.Senior.Media;

/// <summary>
/// A deterministic render projection for one timed-text position. The projection is immutable
/// so a template can render it without observing a document that is being replaced by a provider.
/// </summary>
public sealed record TimedTextProjection(
    TimedTextDocument Document,
    TimedTextTrack Track,
    TimeSpan EffectivePosition,
    TimedTextSegment? ActiveSegment,
    TimedTextWord? ActiveWord,
    double SegmentProgress,
    double WordProgress,
    string? TranslationText,
    ImmutableArray<TimedTextSegment> ContextSegments)
{
    public bool HasActiveSegment => ActiveSegment is not null;
    public bool HasActiveWord => ActiveWord is not null;
    public bool IsTranslationVisible => !string.IsNullOrWhiteSpace(TranslationText);
}

/// <summary>
/// Shared time-domain projection used by TimedTextView and future ASR/translation adapters.
/// It only consumes immutable models; it does not parse files or call a provider.
/// </summary>
public static class TimedTextProjectionCalculator
{
    public static TimedTextProjection? Project(
        TimedTextDocument? document,
        string? trackId,
        TimeSpan position,
        TimeSpan timingOffset = default,
        int contextLineCount = 2)
    {
        var normalized = TimedTextNormalizer.Normalize(document);
        if (normalized is null || normalized.Tracks.Count == 0)
        {
            return null;
        }

        var track = SelectTrack(normalized, trackId);
        if (track is null)
        {
            return null;
        }

        var effectivePosition = AddWithSaturation(position, timingOffset);
        var segment = FindActiveSegment(track.Segments, effectivePosition);
        var word = segment is null ? null : FindActiveWord(segment.Words, effectivePosition);
        var segmentProgress = GetProgress(segment?.Start, segment?.End, effectivePosition);
        var wordProgress = GetProgress(word?.Start, word?.End, effectivePosition);
        var translation = ResolveTranslation(normalized, track, segment, effectivePosition);
        var context = BuildContext(track.Segments, segment, Math.Clamp(contextLineCount, 0, 10));

        return new TimedTextProjection(
            normalized,
            track,
            effectivePosition,
            segment,
            word,
            segmentProgress,
            wordProgress,
            translation,
            context);
    }

    public static TimedTextSegment? FindActiveSegment(IReadOnlyList<TimedTextSegment>? segments, TimeSpan position)
    {
        if (segments is null || segments.Count == 0)
        {
            return null;
        }

        // Segments use half-open intervals. In an overlap, the most recently started cue wins.
        return segments
            .Where(segment => segment.Start <= position && position < segment.End)
            .OrderByDescending(segment => segment.Start)
            .ThenBy(segment => segment.End)
            .FirstOrDefault();
    }

    public static TimedTextWord? FindActiveWord(IReadOnlyList<TimedTextWord>? words, TimeSpan position)
    {
        if (words is null || words.Count == 0)
        {
            return null;
        }

        return words
            .Where(word => word.Start <= position && position < word.End)
            .OrderByDescending(word => word.Start)
            .ThenBy(word => word.End)
            .FirstOrDefault();
    }

    public static double GetProgress(TimeSpan? start, TimeSpan? end, TimeSpan position)
    {
        if (start is null || end is null || end <= start)
        {
            return 0;
        }

        var elapsed = position - start.Value;
        var duration = end.Value - start.Value;
        return Math.Clamp(elapsed.TotalMilliseconds / duration.TotalMilliseconds, 0, 1);
    }

    private static TimedTextTrack? SelectTrack(TimedTextDocument document, string? requestedTrackId)
    {
        if (!string.IsNullOrWhiteSpace(requestedTrackId))
        {
            var explicitTrack = document.Tracks.FirstOrDefault(track =>
                StringComparer.Ordinal.Equals(track.Id, requestedTrackId));
            if (explicitTrack is not null)
            {
                return explicitTrack;
            }
        }

        var culture = System.Globalization.CultureInfo.CurrentUICulture;
        return document.Tracks.FirstOrDefault(track =>
                   !string.IsNullOrWhiteSpace(track.Language) &&
                   (culture.Name.StartsWith(track.Language, StringComparison.OrdinalIgnoreCase) ||
                    track.Language.StartsWith(culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase)))
               ?? document.Tracks.FirstOrDefault();
    }

    private static string? ResolveTranslation(
        TimedTextDocument document,
        TimedTextTrack activeTrack,
        TimedTextSegment? activeSegment,
        TimeSpan position)
    {
        if (activeSegment is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(activeSegment.TranslatedText))
        {
            return activeSegment.TranslatedText;
        }

        // Prefer a translation track that overlaps the active cue and contains the effective
        // position. If there are several tracks, preserve document order for deterministic UI.
        return document.Tracks
            .Where(track => track.Role == TimedTextTrackRole.Translation &&
                            !ReferenceEquals(track, activeTrack))
            .SelectMany(track => track.Segments)
            .Where(segment => segment.Start < activeSegment.End && segment.End > activeSegment.Start)
            .OrderByDescending(segment => segment.Start <= position && position < segment.End)
            .ThenBy(segment => Math.Abs((segment.Start - activeSegment.Start).Ticks))
            .Select(segment => segment.Text)
            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));
    }

    private static ImmutableArray<TimedTextSegment> BuildContext(
        IReadOnlyList<TimedTextSegment> segments,
        TimedTextSegment? activeSegment,
        int contextLineCount)
    {
        if (segments.Count == 0)
        {
            return ImmutableArray<TimedTextSegment>.Empty;
        }

        var activeIndex = -1;
        if (activeSegment is not null)
        {
            for (var index = 0; index < segments.Count; index++)
            {
                if (ReferenceEquals(segments[index], activeSegment) ||
                    StringComparer.Ordinal.Equals(segments[index].Id, activeSegment.Id))
                {
                    activeIndex = index;
                    break;
                }
            }
        }
        if (activeIndex < 0)
        {
            return segments.Take(Math.Max(1, contextLineCount)).ToImmutableArray();
        }

        var start = Math.Max(0, activeIndex - contextLineCount);
        var count = Math.Min(segments.Count - start, contextLineCount * 2 + 1);
        return segments.Skip(start).Take(count).ToImmutableArray();
    }

    private static TimeSpan AddWithSaturation(TimeSpan left, TimeSpan right)
    {
        try
        {
            return left + right;
        }
        catch (OverflowException)
        {
            return right >= TimeSpan.Zero ? TimeSpan.MaxValue : TimeSpan.MinValue;
        }
    }
}
