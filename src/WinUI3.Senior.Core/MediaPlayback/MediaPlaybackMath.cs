namespace WinUI3.Senior.Core;

/// <summary>Deterministic helpers shared by playback controls and fake sessions.</summary>
public static class MediaPlaybackMath
{
    /// <summary>Clamps and merges ranges into sorted, non-empty intervals.</summary>
    public static IReadOnlyList<MediaPlaybackTimeRange> NormalizeRanges(
        IEnumerable<MediaPlaybackTimeRange>? ranges,
        MediaPlaybackTimeRange domain)
    {
        if (ranges is null || !domain.IsValid)
        {
            return Array.Empty<MediaPlaybackTimeRange>();
        }

        var normalized = new List<MediaPlaybackTimeRange>();
        foreach (var range in ranges)
        {
            var start = range.Start < domain.Start ? domain.Start : range.Start;
            var end = range.End > domain.End ? domain.End : range.End;
            if (end > start)
            {
                normalized.Add(new MediaPlaybackTimeRange(start, end));
            }
        }

        normalized.Sort(static (left, right) => left.Start.CompareTo(right.Start));
        for (var index = 1; index < normalized.Count;)
        {
            var previous = normalized[index - 1];
            var current = normalized[index];
            if (current.Start > previous.End)
            {
                index++;
                continue;
            }

            normalized[index - 1] = new MediaPlaybackTimeRange(
                previous.Start,
                current.End > previous.End ? current.End : previous.End);
            normalized.RemoveAt(index);
        }

        return normalized;
    }

    /// <summary>Moves a position out of disabled intervals using the nearest boundary.</summary>
    public static TimeSpan CoercePosition(
        TimeSpan position,
        MediaPlaybackTimeRange domain,
        IEnumerable<MediaPlaybackTimeRange>? disabledRanges,
        int direction = 1)
    {
        var candidate = domain.Clamp(position);
        foreach (var range in NormalizeRanges(disabledRanges, domain))
        {
            if (!range.Contains(candidate))
            {
                continue;
            }

            var before = candidate - range.Start;
            var after = range.End - candidate;
            return after < before || (after == before && direction >= 0) ? range.End : range.Start;
        }

        return candidate;
    }

    /// <summary>Returns the live edge while honoring the requested tolerance.</summary>
    public static TimeSpan GetLiveEdge(MediaPlaybackTimeRange seekableRange, TimeSpan tolerance)
    {
        if (!seekableRange.IsValid)
        {
            return TimeSpan.Zero;
        }

        var safeTolerance = tolerance < TimeSpan.Zero ? TimeSpan.Zero : tolerance;
        var edge = seekableRange.End - safeTolerance;
        return edge < seekableRange.Start ? seekableRange.Start : edge;
    }

    /// <summary>Advances a position by elapsed wall time and playback rate, clamped to the domain.</summary>
    public static TimeSpan Advance(
        TimeSpan position,
        TimeSpan elapsed,
        double playbackRate,
        MediaPlaybackTimeRange domain)
    {
        if (!domain.IsValid || elapsed <= TimeSpan.Zero || !double.IsFinite(playbackRate) || playbackRate <= 0)
        {
            return domain.Clamp(position);
        }

        var deltaTicks = elapsed.Ticks * playbackRate;
        var safeDelta = deltaTicks >= long.MaxValue ? long.MaxValue : deltaTicks <= 0 ? 0 : (long)deltaTicks;
        var candidate = position + TimeSpan.FromTicks(safeDelta);
        return domain.Clamp(candidate);
    }
}
