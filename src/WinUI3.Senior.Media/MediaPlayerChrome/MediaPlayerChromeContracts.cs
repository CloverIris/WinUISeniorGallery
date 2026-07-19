using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

/// <summary>Controls the amount of chrome presented by <see cref="MediaPlayerChrome"/>.</summary>
public enum MediaChromeDisplayMode
{
    Full,
    Compact,
    Minimal,
}

/// <summary>Identifies a host-owned presentation change requested by the control.</summary>
public enum MediaChromePresentationMode
{
    Inline,
    FullScreen,
    CompactOverlay,
}

/// <summary>Supplies a host-owned presentation request.</summary>
public sealed class MediaChromePresentationRequestedEventArgs(MediaChromePresentationMode requestedMode) : EventArgs
{
    public MediaChromePresentationMode RequestedMode { get; } = requestedMode;
    public bool Handled { get; set; }
}

/// <summary>Reports the outcome of a user-originated playback command.</summary>
public sealed class MediaChromeCommandCompletedEventArgs(MediaPlaybackCommandResult result) : EventArgs
{
    public MediaPlaybackCommandResult Result { get; } = result ?? throw new ArgumentNullException(nameof(result));
}
