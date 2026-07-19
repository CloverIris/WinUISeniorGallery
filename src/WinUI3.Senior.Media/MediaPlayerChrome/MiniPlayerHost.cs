using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

/// <summary>
/// A compact, host-owned playback surface. It can be embedded in a page or shell;
/// it never creates a window and never migrates media content by itself.
/// </summary>
[TemplatePart(Name = "PART_RootGrid", Type = typeof(Grid))]
[TemplatePart(Name = "PART_PlayPauseButton", Type = typeof(ButtonBase))]
[TemplatePart(Name = "PART_DismissButton", Type = typeof(ButtonBase))]
[TemplatePart(Name = "PART_RestoreButton", Type = typeof(ButtonBase))]
public sealed class MiniPlayerHost : ContentControl
{
    private readonly SemaphoreSlim _commandGate = new(1, 1);
    private IMediaPlaybackSession? _subscribedSession;
    private ButtonBase? _playPauseButton;
    private ButtonBase? _dismissButton;
    private ButtonBase? _restoreButton;
    private bool _applyingSnapshot;
    private bool _sessionCanPlayPause;
    private PlaybackSessionId? _appliedSessionId;
    private long _appliedRevision = -1;
    private CancellationTokenSource? _commandCancellation;

    public static readonly DependencyProperty PlaybackSessionProperty = DependencyProperty.Register(
        nameof(PlaybackSession), typeof(IMediaPlaybackSession), typeof(MiniPlayerHost), new PropertyMetadata(null, OnSessionChanged));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded), typeof(bool), typeof(MiniPlayerHost), new PropertyMetadata(false, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsDismissibleProperty = DependencyProperty.Register(
        nameof(IsDismissible), typeof(bool), typeof(MiniPlayerHost), new PropertyMetadata(true, OnVisualPropertyChanged));
    public static readonly DependencyProperty ShowRestoreButtonProperty = DependencyProperty.Register(
        nameof(ShowRestoreButton), typeof(bool), typeof(MiniPlayerHost), new PropertyMetadata(true, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsHostWindowActiveProperty = DependencyProperty.Register(
        nameof(IsHostWindowActive), typeof(bool), typeof(MiniPlayerHost), new PropertyMetadata(true, OnVisualPropertyChanged));
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(MiniPlayerHost), new PropertyMetadata(null));
    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        nameof(Subtitle), typeof(string), typeof(MiniPlayerHost), new PropertyMetadata(null));
    public static readonly DependencyProperty QueueCountProperty = DependencyProperty.Register(
        nameof(QueueCount), typeof(int), typeof(MiniPlayerHost), new PropertyMetadata(0));

    public MiniPlayerHost()
    {
        DefaultStyleKey = typeof(MiniPlayerHost);
        Loaded += OnLoaded;
        Unloaded += (_, _) => DetachSession();
    }

    public IMediaPlaybackSession? PlaybackSession
    {
        get => (IMediaPlaybackSession?)GetValue(PlaybackSessionProperty);
        set => SetValue(PlaybackSessionProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public bool IsDismissible
    {
        get => (bool)GetValue(IsDismissibleProperty);
        set => SetValue(IsDismissibleProperty, value);
    }

    public bool ShowRestoreButton
    {
        get => (bool)GetValue(ShowRestoreButtonProperty);
        set => SetValue(ShowRestoreButtonProperty, value);
    }

    public bool IsHostWindowActive
    {
        get => (bool)GetValue(IsHostWindowActiveProperty);
        set => SetValue(IsHostWindowActiveProperty, value);
    }

    public MediaPlaybackSnapshot? CurrentSnapshot { get; private set; }

    public event EventHandler? DismissRequested;
    public event EventHandler? RestoreRequested;
    public event EventHandler? PlayPauseRequested;
    public event EventHandler<MediaChromeCommandCompletedEventArgs>? CommandCompleted;

    /// <summary>Current position copied from the host session for lightweight status presenters.</summary>
    public TimeSpan CurrentPosition { get; private set; }

    /// <summary>Current seekable end, or zero for an unavailable session.</summary>
    public TimeSpan Duration { get; private set; }

    public bool IsLive { get; private set; }

    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Subtitle
    {
        get => (string?)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public int QueueCount
    {
        get => (int)GetValue(QueueCountProperty);
        set => SetValue(QueueCountProperty, value < 0 ? 0 : value);
    }

    public Task<MediaPlaybackCommandResult> TogglePlaybackAsync(CancellationToken cancellationToken = default)
    {
        var session = PlaybackSession;
        if (session is null) return Task.FromResult(MediaPlaybackCommandResult.Rejected());
        return ExecuteToggleAsync(session, cancellationToken);
    }

    public void RequestDismiss()
    {
        if (IsDismissible) DismissRequested?.Invoke(this, EventArgs.Empty);
    }

    public void RequestRestore() => RestoreRequested?.Invoke(this, EventArgs.Empty);

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    protected override void OnApplyTemplate()
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate();
        _playPauseButton = GetTemplateChild("PART_PlayPauseButton") as ButtonBase;
        _dismissButton = GetTemplateChild("PART_DismissButton") as ButtonBase;
        _restoreButton = GetTemplateChild("PART_RestoreButton") as ButtonBase;

        if (_playPauseButton is not null) _playPauseButton.Click += OnPlayPauseClick;
        if (_dismissButton is not null) _dismissButton.Click += OnDismissClick;
        if (_restoreButton is not null) _restoreButton.Click += OnRestoreClick;
        ApplySnapshot(PlaybackSession?.CurrentSnapshot);
    }

    private static void OnSessionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((MiniPlayerHost)sender).AttachSession(args.NewValue as IMediaPlaybackSession);

    private static void OnVisualPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _) =>
        ((MiniPlayerHost)sender).UpdateVisualState();

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

        _subscribedSession = session;
        session.SnapshotChanged += OnSnapshotChanged;
        ApplySnapshot(session.CurrentSnapshot);
    }

    private void DetachSession()
    {
        _commandCancellation?.Cancel();
        _commandCancellation?.Dispose();
        _commandCancellation = null;
        if (_subscribedSession is not null)
        {
            _subscribedSession.SnapshotChanged -= OnSnapshotChanged;
            _subscribedSession = null;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        // Unload detaches native/session events to avoid retaining a page. Reattach
        // the unchanged dependency-property session when the host returns.
        AttachSession(PlaybackSession);
    }

    private void OnSnapshotChanged(object? sender, MediaPlaybackSessionChangedEventArgs args)
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
        CurrentSnapshot = snapshot;
        CurrentPosition = snapshot?.Position ?? TimeSpan.Zero;
        Duration = snapshot?.SeekableRange.End ?? TimeSpan.Zero;
        IsLive = snapshot?.Mode is MediaPlaybackMode.Live or MediaPlaybackMode.LiveDvr;
        _applyingSnapshot = true;
        try
        {
            IsEnabled = snapshot is not null && snapshot.State != MediaPlaybackState.Unavailable;
            _sessionCanPlayPause = false;
            if (_playPauseButton is not null)
            {
                var isPlaying = snapshot?.State is MediaPlaybackState.Playing or MediaPlaybackState.Buffering;
                _playPauseButton.Content = isPlaying ? "Pause" : "Play";
                _sessionCanPlayPause = snapshot?.Capabilities.HasFlag(isPlaying ? MediaPlaybackCapabilities.Pause : MediaPlaybackCapabilities.Play) == true;
                _playPauseButton.IsEnabled = _sessionCanPlayPause && IsHostWindowActive;
            }
        }
        finally
        {
            _applyingSnapshot = false;
        }

        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (_dismissButton is not null) _dismissButton.IsEnabled = IsDismissible;
        if (_restoreButton is not null) _restoreButton.Visibility = ShowRestoreButton ? Visibility.Visible : Visibility.Collapsed;
        if (_playPauseButton is not null) _playPauseButton.IsEnabled = _sessionCanPlayPause && IsHostWindowActive;
        VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Compact", true);
    }

    private async void OnPlayPauseClick(object sender, RoutedEventArgs args)
    {
        if (_applyingSnapshot || PlaybackSession is not { } session) return;
        PlayPauseRequested?.Invoke(this, EventArgs.Empty);
        await ExecuteToggleAsync(session, CancellationToken.None);
    }

    private async Task<MediaPlaybackCommandResult> ExecuteToggleAsync(IMediaPlaybackSession session, CancellationToken cancellationToken)
    {
        try
        {
            await _commandGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (!ReferenceEquals(session, PlaybackSession))
                {
                    return PublishCommandResult(MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed));
                }

                _commandCancellation?.Cancel();
                _commandCancellation?.Dispose();
                _commandCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var token = _commandCancellation.Token;
                var result = session.CurrentSnapshot.State is MediaPlaybackState.Playing or MediaPlaybackState.Buffering
                    ? await session.PauseAsync(token).ConfigureAwait(false)
                    : await session.PlayAsync(token).ConfigureAwait(false);
                return PublishCommandResult(result);
            }
            finally { _commandGate.Release(); }
        }
        catch (OperationCanceledException)
        {
            return PublishCommandResult(MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed));
        }
        catch
        {
            return PublishCommandResult(MediaPlaybackCommandResult.Failed(MediaPlaybackErrorCode.EngineFailure));
        }
    }

    private MediaPlaybackCommandResult PublishCommandResult(MediaPlaybackCommandResult result)
    {
        CommandCompleted?.Invoke(this, new MediaChromeCommandCompletedEventArgs(result));
        return result;
    }

    private void OnDismissClick(object sender, RoutedEventArgs args)
    {
        RequestDismiss();
    }

    private void OnRestoreClick(object sender, RoutedEventArgs args) => RequestRestore();

    private void DetachTemplateHandlers()
    {
        if (_playPauseButton is not null) _playPauseButton.Click -= OnPlayPauseClick;
        if (_dismissButton is not null) _dismissButton.Click -= OnDismissClick;
        if (_restoreButton is not null) _restoreButton.Click -= OnRestoreClick;
        _playPauseButton = null;
        _dismissButton = null;
        _restoreButton = null;
    }
}
