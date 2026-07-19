namespace WinUI3.Senior.Media;

/// <summary>Searches and navigates an immutable timed-text document without parsing a file format.</summary>
public sealed class TimedTextNavigator
{
    private readonly TimedTextDocument _document;

    public TimedTextNavigator(TimedTextDocument document)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
    }

    public TimedTextDocument Document => _document;

    public IReadOnlyList<TimedTextSearchResult> Search(string query, string? trackId = null, bool includeTranslation = true)
    {
        if (string.IsNullOrWhiteSpace(query)) return Array.Empty<TimedTextSearchResult>();
        var results = new List<TimedTextSearchResult>();
        foreach (var track in _document.Tracks ?? Array.Empty<TimedTextTrack>())
        {
            if (trackId is not null && !StringComparer.Ordinal.Equals(track.Id, trackId)) continue;
            foreach (var segment in track.Segments ?? Array.Empty<TimedTextSegment>())
            {
                var primary = segment.Text ?? string.Empty;
                if (primary.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new TimedTextSearchResult(track, segment, false));
                    continue;
                }

                if (includeTranslation && segment.TranslatedText?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                {
                    results.Add(new TimedTextSearchResult(track, segment, true));
                }
            }
        }

        return results;
    }

    public TimedTextSegment? FindAt(string trackId, TimeSpan position)
    {
        var track = _document.Tracks?.FirstOrDefault(item => StringComparer.Ordinal.Equals(item.Id, trackId));
        return track?.Segments?.FirstOrDefault(segment => position >= segment.Start && position < segment.End);
    }

    public TimedTextSegment? FindNext(string trackId, TimeSpan position)
    {
        var track = _document.Tracks?.FirstOrDefault(item => StringComparer.Ordinal.Equals(item.Id, trackId));
        return track?.Segments?.FirstOrDefault(segment => segment.Start > position);
    }

    public TimedTextSegment? FindPrevious(string trackId, TimeSpan position)
    {
        var track = _document.Tracks?.FirstOrDefault(item => StringComparer.Ordinal.Equals(item.Id, trackId));
        return track?.Segments?.Where(segment => segment.End <= position).LastOrDefault();
    }
}

public sealed record TimedTextSearchResult(TimedTextTrack Track, TimedTextSegment Segment, bool IsTranslationMatch);
