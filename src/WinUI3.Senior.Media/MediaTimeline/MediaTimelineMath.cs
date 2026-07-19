using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

/// <summary>Pure time-domain helpers shared by timeline hosts and custom templates.</summary>
public static class MediaTimelineMath
{
    public static IReadOnlyList<MediaPlaybackTimeRange> NormalizeRanges(
        IEnumerable<MediaPlaybackTimeRange>? ranges,
        TimeSpan minimum,
        TimeSpan maximum,
        bool mergeAdjacent = true)
    {
        if (ranges is null || maximum < minimum) return Array.Empty<MediaPlaybackTimeRange>();
        var result = new List<MediaPlaybackTimeRange>();
        foreach (var range in ranges)
        {
            if (!range.IsValid || range.End < minimum || range.Start > maximum) continue;
            var start = range.Start < minimum ? minimum : range.Start;
            var end = range.End > maximum ? maximum : range.End;
            if (end < start) continue;
            result.Add(new MediaPlaybackTimeRange(start, end));
        }

        if (result.Count <= 1) return result;
        result.Sort(static (a, b) => a.Start.CompareTo(b.Start));
        var merged = new List<MediaPlaybackTimeRange>(result.Count) { result[0] };
        for (var i = 1; i < result.Count; i++)
        {
            var previous = merged[^1];
            var joins = result[i].Start < previous.End || (mergeAdjacent && result[i].Start == previous.End);
            if (joins)
            {
                merged[^1] = new MediaPlaybackTimeRange(previous.Start, result[i].End > previous.End ? result[i].End : previous.End);
            }
            else
            {
                merged.Add(result[i]);
            }
        }
        return merged;
    }

    public static TimeSpan Clamp(TimeSpan position, TimeSpan minimum, TimeSpan maximum)
    {
        if (maximum < minimum) return minimum;
        return position < minimum ? minimum : position > maximum ? maximum : position;
    }

    /// <summary>Moves a candidate out of disabled intervals in the direction of travel.</summary>
    public static TimeSpan CoerceSeekPosition(
        TimeSpan candidate,
        TimeSpan minimum,
        TimeSpan maximum,
        IEnumerable<MediaPlaybackTimeRange>? disabledRanges,
        int direction = 1)
    {
        var value = Clamp(candidate, minimum, maximum);
        var ranges = NormalizeRanges(disabledRanges, minimum, maximum, mergeAdjacent: true);
        foreach (var range in ranges)
        {
            if (value < range.Start || value > range.End) continue;
            if (direction < 0)
            {
                value = range.Start > minimum ? range.Start - TimeSpan.FromTicks(1) : range.Start;
            }
            else
            {
                value = range.End < maximum ? range.End + TimeSpan.FromTicks(1) : range.End;
            }
            break;
        }
        return Clamp(value, minimum, maximum);
    }

    public static TimeSpan GetLiveEdge(TimeSpan minimum, TimeSpan maximum, TimeSpan tolerance)
    {
        if (maximum < minimum) return minimum;
        var safeTolerance = tolerance < TimeSpan.Zero ? TimeSpan.Zero : tolerance;
        var edge = maximum - safeTolerance;
        return edge < minimum ? minimum : edge;
    }

    public static double GetPositionRatio(TimeSpan position, TimeSpan minimum, TimeSpan maximum)
    {
        if (maximum <= minimum) return 0;
        var ratio = (position - minimum).TotalMilliseconds / (maximum - minimum).TotalMilliseconds;
        return Math.Clamp(ratio, 0, 1);
    }

    public static TimeSpan GetPositionFromRatio(double ratio, TimeSpan minimum, TimeSpan maximum)
    {
        if (maximum <= minimum) return minimum;
        var clamped = Math.Clamp(ratio, 0, 1);
        return minimum + TimeSpan.FromTicks((long)((maximum - minimum).Ticks * clamped));
    }

    public static string FormatPosition(TimeSpan position, bool includeHours = false)
    {
        var safe = position < TimeSpan.Zero ? TimeSpan.Zero : position;
        return includeHours || safe.TotalHours >= 1
            ? $"{(int)safe.TotalHours:00}:{safe.Minutes:00}:{safe.Seconds:00}"
            : $"{safe.Minutes:00}:{safe.Seconds:00}";
    }
}
