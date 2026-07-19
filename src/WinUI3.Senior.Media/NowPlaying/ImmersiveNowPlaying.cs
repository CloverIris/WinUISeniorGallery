using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

/// <summary>
/// Hostable Now Playing surface for album art, playback controls, queue and timed-text views.
/// It exposes presentation requests only; window creation and media ownership stay with the host.
/// </summary>
[TemplatePart(Name = "PART_PlayButton", Type = typeof(ButtonBase))]
[TemplatePart(Name = "PART_PreviousButton", Type = typeof(ButtonBase))]
[TemplatePart(Name = "PART_NextButton", Type = typeof(ButtonBase))]
[TemplatePart(Name = "PART_QueueButton", Type = typeof(ButtonBase))]
[TemplatePart(Name = "PART_LyricsButton", Type = typeof(ButtonBase))]
[TemplatePart(Name = "PART_FullScreenButton", Type = typeof(ButtonBase))]
public sealed class ImmersiveNowPlaying : Control
{
    private NowPlayingController? _controller;
    private ButtonBase? _playButton;
    private ButtonBase? _previousButton;
    private ButtonBase? _nextButton;
    private ButtonBase? _queueButton;
    private ButtonBase? _lyricsButton;
    private ButtonBase? _fullScreenButton;
    private bool _isApplyingState;

    public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register(
        nameof(Controller), typeof(NowPlayingController), typeof(ImmersiveNowPlaying), new PropertyMetadata(null, OnControllerChanged));
    public static readonly DependencyProperty PresentationModeProperty = DependencyProperty.Register(
        nameof(PresentationMode), typeof(NowPlayingPresentationMode), typeof(ImmersiveNowPlaying), new PropertyMetadata(NowPlayingPresentationMode.Full, OnPresentationModeChanged));
    public static readonly DependencyProperty IsBackdropEnabledProperty = DependencyProperty.Register(
        nameof(IsBackdropEnabled), typeof(bool), typeof(ImmersiveNowPlaying), new PropertyMetadata(true, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsAutoHideEnabledProperty = DependencyProperty.Register(
        nameof(IsAutoHideEnabled), typeof(bool), typeof(ImmersiveNowPlaying), new PropertyMetadata(true, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsQueueButtonVisibleProperty = DependencyProperty.Register(
        nameof(IsQueueButtonVisible), typeof(bool), typeof(ImmersiveNowPlaying), new PropertyMetadata(true, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsLyricsButtonVisibleProperty = DependencyProperty.Register(
        nameof(IsLyricsButtonVisible), typeof(bool), typeof(ImmersiveNowPlaying), new PropertyMetadata(true, OnVisualPropertyChanged));

    public ImmersiveNowPlaying()
    {
        DefaultStyleKey = typeof(ImmersiveNowPlaying);
        Unloaded += (_, _) => DetachController();
        Loaded += (_, _) => AttachController(Controller);
    }

    public NowPlayingController? Controller
    {
        get => (NowPlayingController?)GetValue(ControllerProperty);
        set => SetValue(ControllerProperty, value);
    }

    public NowPlayingPresentationMode PresentationMode
    {
        get => (NowPlayingPresentationMode)GetValue(PresentationModeProperty);
        set => SetValue(PresentationModeProperty, value);
    }

    public bool IsBackdropEnabled
    {
        get => (bool)GetValue(IsBackdropEnabledProperty);
        set => SetValue(IsBackdropEnabledProperty, value);
    }

    public bool IsAutoHideEnabled
    {
        get => (bool)GetValue(IsAutoHideEnabledProperty);
        set => SetValue(IsAutoHideEnabledProperty, value);
    }

    public bool IsQueueButtonVisible
    {
        get => (bool)GetValue(IsQueueButtonVisibleProperty);
        set => SetValue(IsQueueButtonVisibleProperty, value);
    }

    public bool IsLyricsButtonVisible
    {
        get => (bool)GetValue(IsLyricsButtonVisibleProperty);
        set => SetValue(IsLyricsButtonVisibleProperty, value);
    }

    public NowPlayingState? CurrentState { get; private set; }

    public event EventHandler<MediaChromePresentationRequestedEventArgs>? PresentationRequested;
    public event EventHandler? QueueRequested;
    public event EventHandler? LyricsRequested;

    public Task<MediaPlaybackCommandResult> TogglePlaybackAsync(CancellationToken cancellationToken = default) =>
        Controller?.ToggleAsync(cancellationToken) ?? Task.FromResult(MediaPlaybackCommandResult.Rejected());

    public Task<MediaPlaybackCommandResult> NextAsync(CancellationToken cancellationToken = default) =>
        Controller?.NextAsync(cancellationToken) ?? Task.FromResult(MediaPlaybackCommandResult.Rejected());

    public Task<MediaPlaybackCommandResult> PreviousAsync(CancellationToken cancellationToken = default) =>
        Controller?.PreviousAsync(cancellationToken) ?? Task.FromResult(MediaPlaybackCommandResult.Rejected());

    public void RequestFullScreen() =>
        PresentationRequested?.Invoke(this, new MediaChromePresentationRequestedEventArgs(MediaChromePresentationMode.FullScreen));

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    protected override void OnApplyTemplate()
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate();
        _playButton = GetTemplateChild("PART_PlayButton") as ButtonBase;
        _previousButton = GetTemplateChild("PART_PreviousButton") as ButtonBase;
        _nextButton = GetTemplateChild("PART_NextButton") as ButtonBase;
        _queueButton = GetTemplateChild("PART_QueueButton") as ButtonBase;
        _lyricsButton = GetTemplateChild("PART_LyricsButton") as ButtonBase;
        _fullScreenButton = GetTemplateChild("PART_FullScreenButton") as ButtonBase;

        if (_playButton is not null) _playButton.Click += OnPlayClick;
        if (_previousButton is not null) _previousButton.Click += OnPreviousClick;
        if (_nextButton is not null) _nextButton.Click += OnNextClick;
        if (_queueButton is not null) _queueButton.Click += OnQueueClick;
        if (_lyricsButton is not null) _lyricsButton.Click += OnLyricsClick;
        if (_fullScreenButton is not null) _fullScreenButton.Click += OnFullScreenClick;
        ApplyState(CurrentState);
    }

    private static void OnControllerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((ImmersiveNowPlaying)sender).AttachController(args.NewValue as NowPlayingController);

    private static void OnPresentationModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var control = (ImmersiveNowPlaying)sender;
        control.Controller?.SetPresentationMode((NowPlayingPresentationMode)args.NewValue);
        control.ApplyState(control.CurrentState);
    }

    private static void OnVisualPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _) =>
        ((ImmersiveNowPlaying)sender).ApplyState(((ImmersiveNowPlaying)sender).CurrentState);

    private void AttachController(NowPlayingController? controller)
    {
        DetachController();
        _controller = controller;
        if (_controller is null)
        {
            ApplyState(null);
            return;
        }

        _controller.StateChanged += OnControllerStateChanged;
        ApplyState(_controller.CurrentState);
    }

    private void DetachController()
    {
        if (_controller is not null) _controller.StateChanged -= OnControllerStateChanged;
        _controller = null;
    }

    private void OnControllerStateChanged(object? sender, NowPlayingStateChangedEventArgs args)
    {
        if (!ReferenceEquals(sender, Controller)) return;
        if (DispatcherQueue.HasThreadAccess) ApplyState(args.State);
        else DispatcherQueue.TryEnqueue(() =>
        {
            if (ReferenceEquals(sender, Controller)) ApplyState(args.State);
        });
    }

    private void ApplyState(NowPlayingState? state)
    {
        CurrentState = state;
        _isApplyingState = true;
        try
        {
            IsEnabled = state is not null;
            if (_playButton is not null)
            {
                var isPlaying = state?.Playback?.State is MediaPlaybackState.Playing or MediaPlaybackState.Buffering;
                _playButton.Content = isPlaying ? "Pause" : "Play";
                _playButton.IsEnabled = state?.Playback?.Capabilities.HasFlag(isPlaying ? MediaPlaybackCapabilities.Pause : MediaPlaybackCapabilities.Play) == true;
            }

            if (_queueButton is not null) _queueButton.Visibility = IsQueueButtonVisible ? Visibility.Visible : Visibility.Collapsed;
            if (_lyricsButton is not null) _lyricsButton.Visibility = IsLyricsButtonVisible ? Visibility.Visible : Visibility.Collapsed;
            AutomationProperties.SetName(this, state?.CurrentItem is { } item ? $"Now playing: {item.Title}" : "Now playing");
        }
        finally { _isApplyingState = false; }

        VisualStateManager.GoToState(this, state?.PresentationMode.ToString() ?? PresentationMode.ToString(), true);
    }

    private async void OnPlayClick(object sender, RoutedEventArgs args)
    {
        if (!_isApplyingState) await TogglePlaybackAsync();
    }

    private async void OnPreviousClick(object sender, RoutedEventArgs args) => await PreviousAsync();
    private async void OnNextClick(object sender, RoutedEventArgs args) => await NextAsync();

    private void OnQueueClick(object sender, RoutedEventArgs args) => QueueRequested?.Invoke(this, EventArgs.Empty);
    private void OnLyricsClick(object sender, RoutedEventArgs args) => LyricsRequested?.Invoke(this, EventArgs.Empty);
    private void OnFullScreenClick(object sender, RoutedEventArgs args) => RequestFullScreen();

    private void DetachTemplateHandlers()
    {
        if (_playButton is not null) _playButton.Click -= OnPlayClick;
        if (_previousButton is not null) _previousButton.Click -= OnPreviousClick;
        if (_nextButton is not null) _nextButton.Click -= OnNextClick;
        if (_queueButton is not null) _queueButton.Click -= OnQueueClick;
        if (_lyricsButton is not null) _lyricsButton.Click -= OnLyricsClick;
        if (_fullScreenButton is not null) _fullScreenButton.Click -= OnFullScreenClick;
        _playButton = null;
        _previousButton = null;
        _nextButton = null;
        _queueButton = null;
        _lyricsButton = null;
        _fullScreenButton = null;
    }
}
