using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

/// <summary>A host-controlled, engine-neutral media command surface.</summary>
[TemplatePart(Name = "PART_RootGrid", Type = typeof(Grid))]
[TemplatePart(Name = "PART_Timeline", Type = typeof(MediaTimeline))]
[TemplatePart(Name = "PART_PlayPauseButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_StopButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_VolumeSlider", Type = typeof(Slider))]
[TemplatePart(Name = "PART_FullScreenButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_CompactOverlayButton", Type = typeof(Button))]
public sealed class MediaPlayerChrome : Control
{
    private readonly DispatcherQueueTimer _autoHideTimer;
    private readonly SemaphoreSlim _commandGate = new(1, 1);
    private CancellationTokenSource? _sessionCancellation;
    private CancellationTokenSource? _seekCancellation;
    private IMediaPlaybackSession? _subscribedSession;
    private MediaTimeline? _timeline;
    private Button? _playPauseButton;
    private Button? _stopButton;
    private Slider? _volumeSlider;
    private Button? _fullScreenButton;
    private Button? _compactOverlayButton;
    private bool _isApplyingSnapshot;
    private bool _isPointerOver;
    private MediaPlaybackSnapshot? _currentSnapshot;
    private PlaybackSessionId? _appliedSessionId;
    private long _appliedRevision = -1;

    public static readonly DependencyProperty PlaybackSessionProperty = DependencyProperty.Register(
        nameof(PlaybackSession), typeof(IMediaPlaybackSession), typeof(MediaPlayerChrome), new PropertyMetadata(null, OnPlaybackSessionChanged));
    public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(
        nameof(DisplayMode), typeof(MediaChromeDisplayMode), typeof(MediaPlayerChrome), new PropertyMetadata(MediaChromeDisplayMode.Full));
    public static readonly DependencyProperty IsAutoHideEnabledProperty = DependencyProperty.Register(
        nameof(IsAutoHideEnabled), typeof(bool), typeof(MediaPlayerChrome), new PropertyMetadata(true, OnAutoHideChanged));
    public static readonly DependencyProperty AutoHideDelayProperty = DependencyProperty.Register(
        nameof(AutoHideDelay), typeof(TimeSpan), typeof(MediaPlayerChrome), new PropertyMetadata(TimeSpan.FromSeconds(3), OnAutoHideChanged));
    public static readonly DependencyProperty IsCompactOverlayAvailableProperty = DependencyProperty.Register(
        nameof(IsCompactOverlayAvailable), typeof(bool), typeof(MediaPlayerChrome), new PropertyMetadata(false));
    public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register(
        nameof(IsFullScreen), typeof(bool), typeof(MediaPlayerChrome), new PropertyMetadata(false));
    public static readonly DependencyProperty IsCompactOverlayProperty = DependencyProperty.Register(
        nameof(IsCompactOverlay), typeof(bool), typeof(MediaPlayerChrome), new PropertyMetadata(false));

    public MediaPlayerChrome()
    {
        DefaultStyleKey = typeof(MediaPlayerChrome);
        _autoHideTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _autoHideTimer.Tick += (_, _) => HideChrome();
        PointerEntered += (_, _) => { _isPointerOver = true; ShowChrome(); };
        PointerExited += (_, _) => { _isPointerOver = false; ShowChrome(); };
        PointerMoved += (_, _) => ShowChrome();
        GotFocus += (_, _) => ShowChrome();
        Unloaded += (_, _) => DetachSession();
        Loaded += OnLoaded;
    }

    public IMediaPlaybackSession? PlaybackSession
    {
        get => (IMediaPlaybackSession?)GetValue(PlaybackSessionProperty);
        set => SetValue(PlaybackSessionProperty, value);
    }

    public MediaChromeDisplayMode DisplayMode
    {
        get => (MediaChromeDisplayMode)GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    public bool IsAutoHideEnabled
    {
        get => (bool)GetValue(IsAutoHideEnabledProperty);
        set => SetValue(IsAutoHideEnabledProperty, value);
    }

    public TimeSpan AutoHideDelay
    {
        get => (TimeSpan)GetValue(AutoHideDelayProperty);
        set => SetValue(AutoHideDelayProperty, value);
    }

    public bool IsCompactOverlayAvailable
    {
        get => (bool)GetValue(IsCompactOverlayAvailableProperty);
        set => SetValue(IsCompactOverlayAvailableProperty, value);
    }

    public bool IsFullScreen
    {
        get => (bool)GetValue(IsFullScreenProperty);
        set => SetValue(IsFullScreenProperty, value);
    }

    public bool IsCompactOverlay
    {
        get => (bool)GetValue(IsCompactOverlayProperty);
        set => SetValue(IsCompactOverlayProperty, value);
    }

    public event EventHandler<MediaChromePresentationRequestedEventArgs>? PresentationRequested;
    public event EventHandler<MediaChromeCommandCompletedEventArgs>? CommandCompleted;
    public MediaPlaybackCommandResult? LastCommandResult { get; private set; }
    public MediaPlaybackSnapshot? CurrentSnapshot => _currentSnapshot;
    public event EventHandler<MediaPlaybackSessionChangedEventArgs>? SnapshotApplied;

    /// <summary>Executes play/pause through the current host session without overlapping UI commands.</summary>
    public Task<MediaPlaybackCommandResult> TogglePlaybackAsync(CancellationToken cancellationToken = default)
    {
        var session = PlaybackSession;
        if (session is null)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected());
        }

        return ExecuteCommandAsync(session, (current, token) =>
            current.CurrentSnapshot.State is MediaPlaybackState.Playing or MediaPlaybackState.Buffering
                ? current.PauseAsync(token)
                : current.PlayAsync(token), cancellationToken);
    }

    /// <summary>Requests a playback-rate change while preserving the engine-neutral boundary.</summary>
    public Task<MediaPlaybackCommandResult> SetPlaybackRateAsync(double playbackRate, CancellationToken cancellationToken = default)
    {
        if (!double.IsFinite(playbackRate) || playbackRate <= 0)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument));
        }

        return PlaybackSession is { } session
            ? ExecuteCommandAsync(session, (current, token) => current.SetPlaybackRateAsync(playbackRate, token), cancellationToken)
            : Task.FromResult(MediaPlaybackCommandResult.Rejected());
    }

    public Task<MediaPlaybackCommandResult> SetVolumeAsync(double volume, CancellationToken cancellationToken = default)
    {
        if (!double.IsFinite(volume) || volume is < 0 or > 1)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument));
        }

        return PlaybackSession is { } session
            ? ExecuteCommandAsync(session, (current, token) => current.SetVolumeAsync(volume, token), cancellationToken)
            : Task.FromResult(MediaPlaybackCommandResult.Rejected());
    }

    public Task<MediaPlaybackCommandResult> SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
    {
        if (position < TimeSpan.Zero)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument));
        }

        if (PlaybackSession is not { } session)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected());
        }

        // Scrubbing may issue a new commit before the previous one has reached
        // the engine. Cancel the superseded request while preserving the command
        // gate for play/stop/volume operations.
        var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var previous = Interlocked.Exchange(ref _seekCancellation, linked);
        previous?.Cancel();
        previous?.Dispose();
        return SeekWithCancellationAsync(session, position, linked);
    }

    public Task<MediaPlaybackCommandResult> StopAsync(CancellationToken cancellationToken = default) =>
        PlaybackSession is { } session
            ? ExecuteCommandAsync(session, static (current, token) => current.StopAsync(token), cancellationToken)
            : Task.FromResult(MediaPlaybackCommandResult.Rejected());

    /// <summary>Raises a host-owned presentation request without changing the control's state.</summary>
    public void RequestPresentation(MediaChromePresentationMode mode) =>
        PresentationRequested?.Invoke(this, new MediaChromePresentationRequestedEventArgs(mode));

    protected override void OnApplyTemplate()
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate();
        _timeline = GetTemplateChild("PART_Timeline") as MediaTimeline;
        _playPauseButton = GetTemplateChild("PART_PlayPauseButton") as Button;
        _stopButton = GetTemplateChild("PART_StopButton") as Button;
        _volumeSlider = GetTemplateChild("PART_VolumeSlider") as Slider;
        _fullScreenButton = GetTemplateChild("PART_FullScreenButton") as Button;
        _compactOverlayButton = GetTemplateChild("PART_CompactOverlayButton") as Button;

        if (_playPauseButton is not null) _playPauseButton.Click += OnPlayPauseClick;
        if (_stopButton is not null) _stopButton.Click += OnStopClick;
        if (_volumeSlider is not null) _volumeSlider.ValueChanged += OnVolumeChanged;
        if (_timeline is not null)
        {
            _timeline.SeekRequested += OnTimelineSeekRequested;
            _timeline.LiveEdgeRequested += OnTimelineLiveEdgeRequested;
        }
        if (_fullScreenButton is not null) _fullScreenButton.Click += OnFullScreenClick;
        if (_compactOverlayButton is not null) _compactOverlayButton.Click += OnCompactOverlayClick;
        ApplySnapshot(PlaybackSession?.CurrentSnapshot);
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new MediaPlayerChromeAutomationPeer(this);

    private static void OnPlaybackSessionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((MediaPlayerChrome)sender).AttachSession(args.NewValue as IMediaPlaybackSession);

    private static void OnAutoHideChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((MediaPlayerChrome)sender).ShowChrome();

    private void AttachSession(IMediaPlaybackSession? session)
    {
        DetachSession();
        _appliedSessionId = null;
        _appliedRevision = -1;
        if (session is null)
        {
            ApplySnapshot(null);
            return;
        }

        _sessionCancellation = new CancellationTokenSource();
        _subscribedSession = session;
        session.SnapshotChanged += OnSessionSnapshotChanged;
        ApplySnapshot(session.CurrentSnapshot);
    }

    private void DetachSession()
    {
        _autoHideTimer.Stop();
        var seekCancellation = Interlocked.Exchange(ref _seekCancellation, null);
        seekCancellation?.Cancel();
        seekCancellation?.Dispose();
        _sessionCancellation?.Cancel();
        _sessionCancellation?.Dispose();
        _sessionCancellation = null;
        if (_subscribedSession is not null)
        {
            _subscribedSession.SnapshotChanged -= OnSessionSnapshotChanged;
            _subscribedSession = null;
        }
    }

    private async Task<MediaPlaybackCommandResult> SeekWithCancellationAsync(
        IMediaPlaybackSession session,
        TimeSpan position,
        CancellationTokenSource linked)
    {
        try
        {
            return await ExecuteCommandAsync(session, (current, token) => current.SeekAsync(position, token), linked.Token);
        }
        finally
        {
            Interlocked.CompareExchange(ref _seekCancellation, null, linked);
            linked.Dispose();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        AttachSession(PlaybackSession);
    }

    private void OnSessionSnapshotChanged(object? sender, MediaPlaybackSessionChangedEventArgs args)
    {
        if (!ReferenceEquals(sender, PlaybackSession)) return;
        if (DispatcherQueue.HasThreadAccess) ApplySnapshot(args.Snapshot);
        else DispatcherQueue.TryEnqueue(() =>
        {
            if (ReferenceEquals(sender, PlaybackSession)) ApplySnapshot(args.Snapshot);
        });
    }

    private void ApplySnapshot(MediaPlaybackSnapshot? snapshot)
    {
        // Dispatcher delivery can reorder events when a host swaps sessions. A revision
        // is comparable only inside one session; a new session always resets the fence.
        if (snapshot is not null && _appliedSessionId is { } appliedSession &&
            appliedSession.Equals(snapshot.SessionId) && snapshot.Revision < _appliedRevision)
        {
            return;
        }
        if (snapshot is not null)
        {
            _appliedSessionId = snapshot.SessionId;
            _appliedRevision = snapshot.Revision;
        }
        _currentSnapshot = snapshot;
        _isApplyingSnapshot = true;
        try
        {
            IsEnabled = snapshot is not null;
            if (snapshot is null)
            {
                if (_playPauseButton is not null) _playPauseButton.Content = "Play";
                return;
            }

            if (_playPauseButton is not null)
            {
                _playPauseButton.Content = snapshot.State is WinUI3.Senior.Core.MediaPlaybackState.Playing or WinUI3.Senior.Core.MediaPlaybackState.Buffering ? "Pause" : "Play";
                _playPauseButton.IsEnabled = snapshot.Capabilities.HasFlag(snapshot.State == WinUI3.Senior.Core.MediaPlaybackState.Playing ? MediaPlaybackCapabilities.Pause : MediaPlaybackCapabilities.Play);
            }
            if (_stopButton is not null) _stopButton.IsEnabled = snapshot.Capabilities.HasFlag(MediaPlaybackCapabilities.Stop);
            if (_volumeSlider is not null)
            {
                _volumeSlider.IsEnabled = snapshot.Capabilities.HasFlag(MediaPlaybackCapabilities.Volume);
                _volumeSlider.Value = snapshot.Volume * 100;
            }
            if (_timeline is not null)
            {
                _timeline.Mode = snapshot.Mode switch
                {
                    MediaPlaybackMode.Live => MediaTimelineMode.Live,
                    MediaPlaybackMode.LiveDvr => MediaTimelineMode.LiveDvr,
                    _ => MediaTimelineMode.VideoOnDemand,
                };
                _timeline.Minimum = snapshot.SeekableRange.Start;
                _timeline.Maximum = snapshot.SeekableRange.End;
                _timeline.Position = snapshot.Position;
                _timeline.BufferedRanges = snapshot.BufferedRanges;
                _timeline.IsSeekEnabled = snapshot.Capabilities.HasFlag(MediaPlaybackCapabilities.Seek);
                _timeline.PlaybackRate = snapshot.PlaybackRate;
            }
        }
        finally { _isApplyingSnapshot = false; }

        if (snapshot is not null) SnapshotApplied?.Invoke(this, new MediaPlaybackSessionChangedEventArgs(snapshot));

        if (snapshot?.State == WinUI3.Senior.Core.MediaPlaybackState.Playing) ShowChrome();
        else _autoHideTimer.Stop();
    }

    private async void OnPlayPauseClick(object sender, RoutedEventArgs e)
    {
        await TogglePlaybackAsync(_sessionCancellation?.Token ?? CancellationToken.None);
    }

    private async void OnStopClick(object sender, RoutedEventArgs e)
    {
        if (PlaybackSession is { } session)
        {
            await ExecuteCommandAsync(session, static (current, token) => current.StopAsync(token), _sessionCancellation?.Token ?? CancellationToken.None);
        }
    }

    private async void OnVolumeChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (_isApplyingSnapshot || PlaybackSession is not { } session) return;
        await ExecuteCommandAsync(session, (current, token) => current.SetVolumeAsync(e.NewValue / 100d, token), _sessionCancellation?.Token ?? CancellationToken.None);
    }

    private void OnFullScreenClick(object sender, RoutedEventArgs e) => RequestPresentation(IsFullScreen ? MediaChromePresentationMode.Inline : MediaChromePresentationMode.FullScreen);

    private async void OnTimelineSeekRequested(object? sender, MediaTimelineSeekRequestedEventArgs e)
    {
        if (PlaybackSession is not null)
        {
            await SeekAsync(e.Position, _sessionCancellation?.Token ?? CancellationToken.None);
        }
    }

    private async void OnTimelineLiveEdgeRequested(object? sender, EventArgs e)
    {
        if (PlaybackSession is { } session)
        {
            await SeekAsync(session.CurrentSnapshot.SeekableRange.End, _sessionCancellation?.Token ?? CancellationToken.None);
        }
    }

    private void OnCompactOverlayClick(object sender, RoutedEventArgs e)
    {
        if (IsCompactOverlayAvailable) RequestPresentation(IsCompactOverlay ? MediaChromePresentationMode.Inline : MediaChromePresentationMode.CompactOverlay);
    }

    private void ShowChrome()
    {
        Opacity = 1;
        _autoHideTimer.Stop();
        if (IsAutoHideEnabled && PlaybackSession?.CurrentSnapshot.State == WinUI3.Senior.Core.MediaPlaybackState.Playing && !FocusState.Equals(FocusState.Keyboard))
        {
            _autoHideTimer.Interval = AutoHideDelay < TimeSpan.FromSeconds(1) ? TimeSpan.FromSeconds(1) : AutoHideDelay;
            _autoHideTimer.Start();
        }
    }

    private void HideChrome()
    {
        _autoHideTimer.Stop();
        if (PlaybackSession?.CurrentSnapshot.State == WinUI3.Senior.Core.MediaPlaybackState.Playing && !_isPointerOver && FocusState == FocusState.Unfocused) Opacity = 0.08;
    }

    private void DetachTemplateHandlers()
    {
        if (_playPauseButton is not null) _playPauseButton.Click -= OnPlayPauseClick;
        if (_stopButton is not null) _stopButton.Click -= OnStopClick;
        if (_volumeSlider is not null) _volumeSlider.ValueChanged -= OnVolumeChanged;
        if (_timeline is not null)
        {
            _timeline.SeekRequested -= OnTimelineSeekRequested;
            _timeline.LiveEdgeRequested -= OnTimelineLiveEdgeRequested;
        }
        if (_fullScreenButton is not null) _fullScreenButton.Click -= OnFullScreenClick;
        if (_compactOverlayButton is not null) _compactOverlayButton.Click -= OnCompactOverlayClick;
    }

    private async Task<MediaPlaybackCommandResult> ExecuteCommandAsync(
        IMediaPlaybackSession session,
        Func<IMediaPlaybackSession, CancellationToken, Task<MediaPlaybackCommandResult>> command,
        CancellationToken cancellationToken)
    {
        try
        {
            await _commandGate.WaitAsync(cancellationToken);
            try
            {
                if (!ReferenceEquals(session, PlaybackSession))
                {
                    return MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed);
                }

                var result = await command(session, cancellationToken);
                LastCommandResult = result;
                CommandCompleted?.Invoke(this, new MediaChromeCommandCompletedEventArgs(result));
                return result;
            }
            finally
            {
                _commandGate.Release();
            }
        }
        catch (OperationCanceledException)
        {
            var result = MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed);
            LastCommandResult = result;
            CommandCompleted?.Invoke(this, new MediaChromeCommandCompletedEventArgs(result));
            return result;
        }
        catch
        {
            var result = MediaPlaybackCommandResult.Failed(MediaPlaybackErrorCode.EngineFailure);
            LastCommandResult = result;
            CommandCompleted?.Invoke(this, new MediaChromeCommandCompletedEventArgs(result));
            return result;
        }
    }
}

internal sealed class MediaPlayerChromeAutomationPeer(MediaPlayerChrome owner) : FrameworkElementAutomationPeer(owner), IInvokeProvider
{
    private readonly MediaPlayerChrome _owner = owner;

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;
    protected override string GetClassNameCore() => nameof(MediaPlayerChrome);
    protected override string GetNameCore() => AutomationProperties.GetName(Owner) is { Length: > 0 } name ? name : "Media player controls";

    public void Invoke()
    {
        if (!_owner.IsEnabled) throw new ElementNotEnabledException();
        _ = _owner.TogglePlaybackAsync();
    }
}
