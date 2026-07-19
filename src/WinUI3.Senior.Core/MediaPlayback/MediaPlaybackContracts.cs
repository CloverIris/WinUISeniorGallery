using System.Collections.Immutable;

namespace WinUI3.Senior.Core;

/// <summary>Identifies a playback session by a stable, value-comparable identifier.</summary>
public readonly record struct PlaybackSessionId
{
    public PlaybackSessionId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A playback session identifier cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}

/// <summary>Describes the engine-independent state of a playback session.</summary>
public enum MediaPlaybackState
{
    Unavailable,
    Loading,
    Ready,
    Playing,
    Paused,
    Buffering,
    Ended,
    Failed,
}

/// <summary>Describes the timing model exposed by a playback session.</summary>
public enum MediaPlaybackMode
{
    Unknown,
    VideoOnDemand,
    Live,
    LiveDvr,
}

/// <summary>Lists commands that a playback session currently supports.</summary>
[Flags]
public enum MediaPlaybackCapabilities
{
    None = 0,
    Play = 1 << 0,
    Pause = 1 << 1,
    Stop = 1 << 2,
    Seek = 1 << 3,
    Volume = 1 << 4,
    PlaybackRate = 1 << 5,
}

/// <summary>Represents a closed range in the shared media-time domain.</summary>
public readonly record struct MediaPlaybackTimeRange(TimeSpan Start, TimeSpan End)
{
    public bool IsValid => End >= Start;

    public bool Contains(TimeSpan position) => IsValid && position >= Start && position <= End;

    public TimeSpan Clamp(TimeSpan position) => !IsValid
        ? position
        : position < Start ? Start : position > End ? End : position;
}

/// <summary>Identifies the observable outcome of an asynchronous playback command.</summary>
public enum MediaPlaybackCommandStatus
{
    Success,
    Rejected,
    Cancelled,
    Failed,
}

/// <summary>Provides stable, content-free diagnostic codes for failed commands.</summary>
public enum MediaPlaybackErrorCode
{
    None,
    Disposed,
    Unsupported,
    InvalidArgument,
    EngineFailure,
    DispatcherUnavailable,
}

/// <summary>Represents the result of one asynchronous playback command.</summary>
public sealed record MediaPlaybackCommandResult(
    MediaPlaybackCommandStatus Status,
    MediaPlaybackErrorCode ErrorCode = MediaPlaybackErrorCode.None,
    string? CorrelationId = null)
{
    public static MediaPlaybackCommandResult Success { get; } = new(MediaPlaybackCommandStatus.Success);

    public static MediaPlaybackCommandResult Rejected(MediaPlaybackErrorCode errorCode = MediaPlaybackErrorCode.Unsupported) =>
        new(MediaPlaybackCommandStatus.Rejected, errorCode);

    public static MediaPlaybackCommandResult Cancelled(MediaPlaybackErrorCode errorCode = MediaPlaybackErrorCode.Disposed) =>
        new(MediaPlaybackCommandStatus.Cancelled, errorCode);

    public static MediaPlaybackCommandResult Failed(MediaPlaybackErrorCode errorCode, string? correlationId = null) =>
        new(MediaPlaybackCommandStatus.Failed, errorCode, correlationId);
}

/// <summary>An immutable, revisioned view of an engine-neutral playback session.</summary>
public sealed record MediaPlaybackSnapshot
{
    public MediaPlaybackSnapshot(
        PlaybackSessionId sessionId,
        long revision,
        MediaPlaybackState state,
        MediaPlaybackMode mode,
        MediaPlaybackCapabilities capabilities,
        TimeSpan position,
        MediaPlaybackTimeRange seekableRange,
        IEnumerable<MediaPlaybackTimeRange>? bufferedRanges,
        double volume,
        double playbackRate,
        MediaPlaybackErrorCode errorCode = MediaPlaybackErrorCode.None,
        string? correlationId = null)
    {
        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision));
        }

        if (string.IsNullOrWhiteSpace(sessionId.Value))
        {
            throw new ArgumentException("A snapshot requires a non-empty playback session identifier.", nameof(sessionId));
        }

        SessionId = sessionId;
        Revision = revision;
        State = state;
        Mode = mode;
        Capabilities = capabilities;
        Position = position;
        SeekableRange = seekableRange;
        BufferedRanges = bufferedRanges?.ToImmutableArray() ?? ImmutableArray<MediaPlaybackTimeRange>.Empty;
        Volume = double.IsFinite(volume) && volume is >= 0 and <= 1 ? volume : throw new ArgumentOutOfRangeException(nameof(volume));
        PlaybackRate = double.IsFinite(playbackRate) && playbackRate > 0 ? playbackRate : throw new ArgumentOutOfRangeException(nameof(playbackRate));
        ErrorCode = errorCode;
        CorrelationId = correlationId;
    }

    public PlaybackSessionId SessionId { get; }
    public long Revision { get; }
    public MediaPlaybackState State { get; }
    public MediaPlaybackMode Mode { get; }
    public MediaPlaybackCapabilities Capabilities { get; }
    public TimeSpan Position { get; }
    public MediaPlaybackTimeRange SeekableRange { get; }
    public ImmutableArray<MediaPlaybackTimeRange> BufferedRanges { get; }
    public double Volume { get; }
    public double PlaybackRate { get; }
    public MediaPlaybackErrorCode ErrorCode { get; }
    public string? CorrelationId { get; }
}

/// <summary>Provides a revisioned snapshot emitted by a playback session.</summary>
public sealed class MediaPlaybackSessionChangedEventArgs : EventArgs
{
    public MediaPlaybackSessionChangedEventArgs(MediaPlaybackSnapshot snapshot) => Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));

    public MediaPlaybackSnapshot Snapshot { get; }
}

/// <summary>
/// Defines the engine-neutral playback contract consumed by Media controls.
/// Commands are asynchronous, honour cancellation, and return outcomes rather than propagating engine failures to UI consumers.
/// </summary>
public interface IMediaPlaybackSession
{
    MediaPlaybackSnapshot CurrentSnapshot { get; }

    event EventHandler<MediaPlaybackSessionChangedEventArgs>? SnapshotChanged;

    Task<MediaPlaybackCommandResult> PlayAsync(CancellationToken cancellationToken = default);
    Task<MediaPlaybackCommandResult> PauseAsync(CancellationToken cancellationToken = default);
    Task<MediaPlaybackCommandResult> StopAsync(CancellationToken cancellationToken = default);
    Task<MediaPlaybackCommandResult> SeekAsync(TimeSpan position, CancellationToken cancellationToken = default);
    Task<MediaPlaybackCommandResult> SetVolumeAsync(double volume, CancellationToken cancellationToken = default);
    Task<MediaPlaybackCommandResult> SetPlaybackRateAsync(double playbackRate, CancellationToken cancellationToken = default);
}
