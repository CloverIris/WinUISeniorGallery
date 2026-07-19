using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Core;
using WinUI3.Senior.Media;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class MediaTimelinePage : Page
{
    private MediaDemoSession? _session;
    private readonly MediaTimelineNavigator _navigator;

    public IReadOnlyList<MediaTimelineMarker> Chapters { get; } =
    [new("chapter-1", TimeSpan.FromSeconds(45), "Chapter 1"), new("chapter-2", TimeSpan.FromSeconds(120), "Chapter 2")];
    public IReadOnlyList<MediaTimelineMarker> Markers { get; } =
    [new("bookmark", TimeSpan.FromSeconds(90), "Bookmark")];
    public IReadOnlyList<MediaPlaybackTimeRange> DisabledRanges { get; } =
    [new(TimeSpan.FromSeconds(70), TimeSpan.FromSeconds(85))];

    public MediaTimelinePage()
    {
        InitializeComponent();
        _navigator = new MediaTimelineNavigator(Chapters, Markers);
        ModeBox.SelectedIndex = 0;
        ReplaceSession(MediaPlaybackMode.VideoOnDemand);
    }

    protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        if (_session is not null) _session.SnapshotChanged -= OnSnapshotChanged;
        _session?.Dispose();
        _session = null;
        base.OnNavigatedFrom(e);
    }

    private void OnModeChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e) => ReplaceSession(ModeBox.SelectedIndex switch
    {
        1 => MediaPlaybackMode.Live,
        2 => MediaPlaybackMode.LiveDvr,
        _ => MediaPlaybackMode.VideoOnDemand,
    });

    private void ReplaceSession(MediaPlaybackMode mode)
    {
        if (_session is not null) { _session.SnapshotChanged -= OnSnapshotChanged; _session.Dispose(); }
        _session = new MediaDemoSession(mode);
        _session.SnapshotChanged += OnSnapshotChanged;
        ApplySnapshot(_session.CurrentSnapshot);
    }

    private async void OnPlayPauseClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_session is null) return;
        if (_session.CurrentSnapshot.State == MediaPlaybackState.Playing) await _session.PauseAsync();
        else await _session.PlayAsync();
    }

    private async void OnSeekRequested(object sender, MediaTimelineSeekRequestedEventArgs e)
    {
        if (_session is not null) await _session.SeekAsync(e.Position);
    }

    private async void OnLiveEdgeRequested(object? sender, EventArgs e)
    {
        if (_session is not null) await _session.SeekAsync(_session.CurrentSnapshot.SeekableRange.End);
    }

    private async void OnPreviousMarkerClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_session is null) return;
        var target = _navigator.FindPrevious(_session.CurrentSnapshot.Position);
        if (target is not null) await _session.SeekAsync(target.Value);
    }

    private async void OnNextMarkerClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_session is null) return;
        var target = _navigator.FindNext(_session.CurrentSnapshot.Position);
        if (target is not null) await _session.SeekAsync(target.Value);
    }

    private void OnSnapshotChanged(object? sender, MediaPlaybackSessionChangedEventArgs e) => ApplySnapshot(e.Snapshot);

    private void ApplySnapshot(MediaPlaybackSnapshot snapshot)
    {
        Timeline.Mode = snapshot.Mode switch { MediaPlaybackMode.Live => MediaTimelineMode.Live, MediaPlaybackMode.LiveDvr => MediaTimelineMode.LiveDvr, _ => MediaTimelineMode.VideoOnDemand };
        Timeline.Minimum = snapshot.SeekableRange.Start;
        Timeline.Maximum = snapshot.SeekableRange.End;
        Timeline.Position = snapshot.Position;
        Timeline.BufferedRanges = snapshot.BufferedRanges;
        Timeline.PlaybackRate = snapshot.PlaybackRate;
        Timeline.IsSeekEnabled = snapshot.Capabilities.HasFlag(MediaPlaybackCapabilities.Seek);
        StatusText.Text = $"{snapshot.State} · {snapshot.Position:mm\\:ss}";
    }
}
