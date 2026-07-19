using System;

namespace WinUI3.Senior.Media;

/// <summary>Defines the media-window semantics represented by <see cref="MediaTimeline"/>.</summary>
public enum MediaTimelineMode
{
    VideoOnDemand,
    Live,
    LiveDvr,
}

/// <summary>Describes a chapter or application-defined marker on a media timeline.</summary>
public sealed record MediaTimelineMarker(string Id, TimeSpan Position, string? Label = null, object? Data = null)
{
    public string Id { get; init; } = string.IsNullOrWhiteSpace(Id)
        ? throw new ArgumentException("A marker id is required.", nameof(Id))
        : Id;
}

/// <summary>Describes an uncommitted user preview position.</summary>
public sealed class MediaTimelinePreviewEventArgs : EventArgs
{
    public MediaTimelinePreviewEventArgs(TimeSpan position, bool isUserInitiated, MediaTimelineInputKind inputKind)
    {
        Position = position;
        IsUserInitiated = isUserInitiated;
        InputKind = inputKind;
    }

    public TimeSpan Position { get; }
    public bool IsUserInitiated { get; }
    public MediaTimelineInputKind InputKind { get; }
}

/// <summary>Describes one synchronous host seek request. The host reports completion by writing <see cref="MediaTimeline.Position"/>.</summary>
public sealed class MediaTimelineSeekRequestedEventArgs : EventArgs
{
    public MediaTimelineSeekRequestedEventArgs(TimeSpan position, bool isUserInitiated, MediaTimelineInputKind inputKind)
    {
        Position = position;
        IsUserInitiated = isUserInitiated;
        InputKind = inputKind;
    }

    public TimeSpan Position { get; }
    public bool IsUserInitiated { get; }
    public MediaTimelineInputKind InputKind { get; }
}

/// <summary>Identifies the input route that changed or requested a timeline position.</summary>
public enum MediaTimelineInputKind
{
    Unknown,
    Mouse,
    Touch,
    Pen,
    Keyboard,
    GameController,
}
