using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3.Senior.Core;
using WinUI3.Senior.Media;

namespace WinUI3_Senior_Gallery.Pages;

/// <summary>Composition-level Now Playing surface backed by a deterministic local session.</summary>
public sealed partial class ImmersiveNowPlayingPage : Page
{
    private MediaDemoSession? _session;

    public ImmersiveNowPlayingPage()
    {
        InitializeComponent();
        Lyrics.Document = CreateDemoDocument();
        CreateSession(MediaPlaybackMode.VideoOnDemand);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ReleaseSession();
        base.OnNavigatedFrom(e);
    }

    private void OnModeChanged(object sender, SelectionChangedEventArgs e)
    {
        var mode = ModeBox.SelectedIndex == 1 ? MediaPlaybackMode.LiveDvr : MediaPlaybackMode.VideoOnDemand;
        CreateSession(mode);
    }

    private void CreateSession(MediaPlaybackMode mode)
    {
        ReleaseSession();
        _session = new MediaDemoSession(mode);
        _session.SnapshotChanged += OnSnapshotChanged;
        Chrome.PlaybackSession = _session;
        Lyrics.Position = _session.CurrentSnapshot.Position;
        StatusText.Text = $"{mode} · revision {_session.CurrentSnapshot.Revision}";
    }

    private void ReleaseSession()
    {
        if (_session is not null)
        {
            _session.SnapshotChanged -= OnSnapshotChanged;
            _session.Dispose();
            _session = null;
        }

        Chrome.PlaybackSession = null;
    }

    private void OnSnapshotChanged(object? sender, MediaPlaybackSessionChangedEventArgs e)
    {
        Lyrics.Position = e.Snapshot.Position;
        StatusText.Text = $"{e.Snapshot.State} · {e.Snapshot.Position:c} · rev {e.Snapshot.Revision}";
    }

    private void OnPresentationRequested(object sender, MediaChromePresentationRequestedEventArgs e) =>
        StatusText.Text = $"Presentation request: {e.RequestedMode}; host action required.";

    private static TimedTextDocument CreateDemoDocument()
    {
        var segments = Enumerable.Range(0, 12).Select(index =>
        {
            var start = TimeSpan.FromSeconds(index * 10);
            var end = start + TimeSpan.FromSeconds(10);
            return new TimedTextSegment(
                $"now-playing-{index}",
                start,
                end,
                $"Synthetic lyric line {index + 1}",
                $"合成歌词第 {index + 1} 行",
                TimedTextRevisionState.Final,
                [new TimedTextWord($"word-{index}", start, end, $"line {index + 1}", null, TimedTextRevisionState.Final)]);
        }).ToArray();
        return new TimedTextDocument("gallery-immersive-now-playing", 1,
            [new TimedTextTrack("lyrics", "en-US", TimedTextTrackRole.Lyrics, segments)]);
    }
}
