using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

/// <summary>Immutable result returned when a timeline marker is selected.</summary>
public sealed record MediaTimelineMarkerSelection(MediaTimelineMarker Marker, TimeSpan Position, bool IsChapter);

/// <summary>
/// Host-neutral chapter/marker navigation. It does not seek a player; callers submit the
/// returned position to their own IMediaPlaybackSession.
/// </summary>
public sealed class MediaTimelineNavigator
{
    private readonly IReadOnlyList<MediaTimelineMarker> _chapters;
    private readonly IReadOnlyList<MediaTimelineMarker> _markers;

    public MediaTimelineNavigator(
        IEnumerable<MediaTimelineMarker>? chapters = null,
        IEnumerable<MediaTimelineMarker>? markers = null,
        TimeSpan? minimum = null,
        TimeSpan? maximum = null)
    {
        Minimum = minimum ?? TimeSpan.Zero;
        Maximum = maximum ?? TimeSpan.MaxValue;
        if (Maximum < Minimum) throw new ArgumentException("Maximum must be greater than or equal to Minimum.", nameof(maximum));
        _chapters = Normalize(chapters);
        _markers = Normalize(markers);
    }

    public TimeSpan Minimum { get; }
    public TimeSpan Maximum { get; }
    public IReadOnlyList<MediaTimelineMarker> Chapters => _chapters;
    public IReadOnlyList<MediaTimelineMarker> Markers => _markers;

    public MediaTimelineMarkerSelection? FindAt(TimeSpan position, bool includeMarkers = true)
    {
        var chapter = _chapters.LastOrDefault(item => item.Position <= position);
        var marker = includeMarkers ? _markers.LastOrDefault(item => item.Position <= position) : null;
        if (chapter is null && marker is null) return null;
        if (marker is null || (chapter is not null && chapter.Position >= marker.Position))
            return chapter is null ? null : new MediaTimelineMarkerSelection(chapter, chapter.Position, true);
        return new MediaTimelineMarkerSelection(marker, marker.Position, false);
    }

    public TimeSpan? FindNext(TimeSpan position, bool includeMarkers = true)
    {
        var candidates = _chapters.Where(item => item.Position > position).Select(item => item.Position);
        if (includeMarkers) candidates = candidates.Concat(_markers.Where(item => item.Position > position).Select(item => item.Position));
        return candidates.OrderBy(value => value).Select(value => (TimeSpan?)value).FirstOrDefault();
    }

    public TimeSpan? FindPrevious(TimeSpan position, bool includeMarkers = true)
    {
        var candidates = _chapters.Where(item => item.Position < position).Select(item => item.Position);
        if (includeMarkers) candidates = candidates.Concat(_markers.Where(item => item.Position < position).Select(item => item.Position));
        return candidates.OrderByDescending(value => value).Select(value => (TimeSpan?)value).FirstOrDefault();
    }

    public TimeSpan CoerceToTimeline(TimeSpan position) => MediaTimelineMath.Clamp(position, Minimum, Maximum);

    private IReadOnlyList<MediaTimelineMarker> Normalize(IEnumerable<MediaTimelineMarker>? source) =>
        (source ?? Array.Empty<MediaTimelineMarker>())
        .Where(marker => marker.Position >= Minimum && marker.Position <= Maximum)
        .OrderBy(marker => marker.Position)
        .ThenBy(marker => marker.Id, StringComparer.Ordinal)
        .GroupBy(marker => marker.Id, StringComparer.Ordinal)
        .Select(group => group.First())
        .ToArray();
}
