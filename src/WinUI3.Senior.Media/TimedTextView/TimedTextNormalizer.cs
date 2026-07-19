using System.Collections.Immutable;

namespace WinUI3.Senior.Media;

/// <summary>Produces the deterministic, input-preserving snapshot consumed by the view.</summary>
internal static class TimedTextNormalizer
{
    public static TimedTextDocument? Normalize(TimedTextDocument? source)
    {
        if (source is null || string.IsNullOrWhiteSpace(source.Id) || source.Revision < 0)
        {
            return null;
        }

        var tracks = ImmutableArray.CreateBuilder<TimedTextTrack>();
        var trackIds = new HashSet<string>(StringComparer.Ordinal);

        foreach (var track in source.Tracks ?? Array.Empty<TimedTextTrack>())
        {
            if (track is null || string.IsNullOrWhiteSpace(track.Id) || !trackIds.Add(track.Id))
            {
                continue;
            }

            var segments = new List<(TimedTextSegment Segment, int InputIndex)>();
            // Segment and word ids are scoped to a track. Translation tracks commonly
            // reuse the source cue ids; rejecting them globally would silently remove
            // valid bilingual content.
            var segmentIds = new HashSet<string>(StringComparer.Ordinal);
            var wordIds = new HashSet<string>(StringComparer.Ordinal);
            var inputIndex = 0;
            foreach (var segment in track.Segments ?? Array.Empty<TimedTextSegment>())
            {
                if (segment is null || string.IsNullOrWhiteSpace(segment.Id) || segment.End <= segment.Start ||
                    !segmentIds.Add(segment.Id))
                {
                    inputIndex++;
                    continue;
                }

                var words = ImmutableArray.CreateBuilder<TimedTextWord>();
                foreach (var word in segment.Words ?? Array.Empty<TimedTextWord>())
                {
                    if (word is null || string.IsNullOrWhiteSpace(word.Id))
                    {
                        continue;
                    }

                    var start = word.Start < segment.Start ? segment.Start : word.Start;
                    var end = word.End > segment.End ? segment.End : word.End;
                    if (end <= start || !wordIds.Add(word.Id))
                    {
                        continue;
                    }

                    words.Add(word with { Start = start, End = end });
                }

                segments.Add((segment with
                {
                    Text = segment.Text ?? string.Empty,
                    Words = words.ToImmutable(),
                }, inputIndex++));
            }

            var sorted = segments
                .OrderBy(item => item.Segment.Start)
                .ThenBy(item => item.Segment.End)
                .ThenBy(item => item.InputIndex)
                .Select(item => item.Segment)
                .ToImmutableArray();

            if (sorted.Length != 0)
            {
                tracks.Add(track with
                {
                    Language = track.Language ?? string.Empty,
                    Segments = sorted,
                });
            }
        }

        return tracks.Count == 0
            ? null
            : source with { Tracks = tracks.ToImmutable() };
    }
}
