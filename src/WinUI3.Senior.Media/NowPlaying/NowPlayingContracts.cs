using System.Collections.Immutable;
using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

/// <summary>Controls which information a Now Playing surface prioritises.</summary>
public enum NowPlayingPresentationMode
{
    Full,
    Compact,
    Lyrics,
    Queue,
}

/// <summary>Controls how the queue advances when a media item reaches its end.</summary>
public enum NowPlayingRepeatMode
{
    Off,
    All,
    One,
}

/// <summary>Describes one item in the host-owned playback queue.</summary>
public sealed record NowPlayingMediaItem
{
    public NowPlayingMediaItem(
        string id,
        string title,
        string? artist = null,
        string? album = null,
        TimeSpan? duration = null,
        Uri? artworkUri = null,
        string? subtitle = null,
        bool isLive = false)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("An item identifier is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("An item title is required.", nameof(title));
        if (duration is { } value && (value < TimeSpan.Zero || value == Timeout.InfiniteTimeSpan))
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        Id = id;
        Title = title;
        Artist = artist;
        Album = album;
        Duration = duration;
        ArtworkUri = artworkUri;
        Subtitle = subtitle;
        IsLive = isLive;
    }

    public string Id { get; }
    public string Title { get; }
    public string? Artist { get; }
    public string? Album { get; }
    public TimeSpan? Duration { get; }
    public Uri? ArtworkUri { get; }
    public string? Subtitle { get; }
    public bool IsLive { get; }
}

/// <summary>Immutable state emitted by <see cref="NowPlayingController"/>.</summary>
public sealed record NowPlayingState
{
    public NowPlayingState(
        long revision,
        NowPlayingMediaItem? currentItem,
        int currentIndex,
        ImmutableArray<NowPlayingMediaItem> queue,
        MediaPlaybackSnapshot? playback,
        NowPlayingPresentationMode presentationMode,
        NowPlayingRepeatMode repeatMode,
        bool isShuffleEnabled,
        bool isQueueOpen,
        bool isBusy,
        bool isAutoAdvanceEnabled,
        string? pendingItemId = null)
    {
        if (revision < 0) throw new ArgumentOutOfRangeException(nameof(revision));
        if (currentIndex < -1 || currentIndex >= queue.Length) throw new ArgumentOutOfRangeException(nameof(currentIndex));
        if (currentItem is null && currentIndex != -1) throw new ArgumentException("An empty current item requires index -1.", nameof(currentIndex));
        if (currentItem is not null && currentIndex < 0) throw new ArgumentException("A current item requires a non-negative index.", nameof(currentIndex));

        Revision = revision;
        CurrentItem = currentItem;
        CurrentIndex = currentIndex;
        Queue = queue;
        Playback = playback;
        PresentationMode = presentationMode;
        RepeatMode = repeatMode;
        IsShuffleEnabled = isShuffleEnabled;
        IsQueueOpen = isQueueOpen;
        IsBusy = isBusy;
        IsAutoAdvanceEnabled = isAutoAdvanceEnabled;
        PendingItemId = pendingItemId;
    }

    public long Revision { get; }
    public NowPlayingMediaItem? CurrentItem { get; }
    public int CurrentIndex { get; }
    public ImmutableArray<NowPlayingMediaItem> Queue { get; }
    public MediaPlaybackSnapshot? Playback { get; }
    public NowPlayingPresentationMode PresentationMode { get; }
    public NowPlayingRepeatMode RepeatMode { get; }
    public bool IsShuffleEnabled { get; }
    public bool IsQueueOpen { get; }
    public bool IsBusy { get; }
    public bool IsAutoAdvanceEnabled { get; }
    public string? PendingItemId { get; }
}

/// <summary>Reports a state change without exposing mutable queue internals.</summary>
public sealed class NowPlayingStateChangedEventArgs(NowPlayingState state) : EventArgs
{
    public NowPlayingState State { get; } = state ?? throw new ArgumentNullException(nameof(state));
}

/// <summary>Identifies a command issued by a Now Playing surface.</summary>
public enum NowPlayingCommandKind
{
    Play,
    Pause,
    Stop,
    Seek,
    Next,
    Previous,
    ToggleShuffle,
    SetRepeatMode,
    SelectQueueItem,
}

/// <summary>Reports the result of a command and the resulting controller revision.</summary>
public sealed class NowPlayingCommandCompletedEventArgs(
    NowPlayingCommandKind kind,
    MediaPlaybackCommandResult result,
    long revision) : EventArgs
{
    public NowPlayingCommandKind Kind { get; } = kind;
    public MediaPlaybackCommandResult Result { get; } = result ?? throw new ArgumentNullException(nameof(result));
    public long Revision { get; } = revision;
}

/// <summary>Requests that the host bind a queue item to a new playback session.</summary>
public sealed class NowPlayingItemActivationRequestedEventArgs(NowPlayingMediaItem item, int index) : EventArgs
{
    public NowPlayingMediaItem Item { get; } = item ?? throw new ArgumentNullException(nameof(item));
    public int Index { get; } = index;
    public bool Handled { get; set; }
}
