using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using WinUI3.Senior.Core;
using Windows.System;
using DispatcherTimer = Microsoft.UI.Dispatching.DispatcherQueueTimer;

namespace WinUI3.Senior.Media;

/// <summary>
/// An engine-neutral timeline that separates scrub preview from a host-owned seek commit.
/// All callbacks and dependency-property mutation occur on the owning XAML dispatcher.
/// </summary>
public sealed class MediaTimeline : Control
{
    private readonly DispatcherTimer _previewTimer;
    private readonly Stopwatch _previewStopwatch = Stopwatch.StartNew();
    private readonly List<MediaPlaybackTimeRange> _normalizedBufferedRanges = [];
    private readonly List<MediaPlaybackTimeRange> _normalizedDisabledRanges = [];
    private bool _isCoercingPosition;
    private bool _isScrubbing;
    private bool _hasPendingPreview;
    private TimeSpan _pendingPreview;
    private TimeSpan _scrubStartPosition;
    private TimeSpan _scrubMinimum;
    private TimeSpan _scrubMaximum;
    private int _scrubDirection;
    private MediaTimelineInputKind _scrubInputKind;
    private Grid? _track;
    private Thumb? _thumb;
    private FrameworkElement? _root;
    private FrameworkElement? _progress;
    private Canvas? _bufferedPresenter;
    private Canvas? _disabledPresenter;
    private Canvas? _chapterPresenter;
    private Canvas? _markerPresenter;
    private ButtonBase? _goLiveButton;
    private TextBlock? _timeText;
    private TextBlock? _playbackRateText;
    private FrameworkElement? _liveBadge;

    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
        nameof(Mode), typeof(MediaTimelineMode), typeof(MediaTimeline), new PropertyMetadata(MediaTimelineMode.VideoOnDemand, OnTimelinePropertyChanged));
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum), typeof(TimeSpan), typeof(MediaTimeline), new PropertyMetadata(TimeSpan.Zero, OnTimelinePropertyChanged));
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum), typeof(TimeSpan), typeof(MediaTimeline), new PropertyMetadata(TimeSpan.Zero, OnTimelinePropertyChanged));
    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        nameof(Position), typeof(TimeSpan), typeof(MediaTimeline), new PropertyMetadata(TimeSpan.Zero, OnPositionChanged));
    public static readonly DependencyProperty LiveWindowEndTimeProperty = DependencyProperty.Register(
        nameof(LiveWindowEndTime), typeof(DateTimeOffset?), typeof(MediaTimeline), new PropertyMetadata(null, OnTimelinePropertyChanged));
    public static readonly DependencyProperty LiveEdgeToleranceProperty = DependencyProperty.Register(
        nameof(LiveEdgeTolerance), typeof(TimeSpan), typeof(MediaTimeline), new PropertyMetadata(TimeSpan.FromSeconds(3), OnTimelinePropertyChanged));
    public static readonly DependencyProperty PlaybackRateProperty = DependencyProperty.Register(
        nameof(PlaybackRate), typeof(double), typeof(MediaTimeline), new PropertyMetadata(1d, OnTimelinePropertyChanged));
    public static readonly DependencyProperty BufferedRangesProperty = DependencyProperty.Register(
        nameof(BufferedRanges), typeof(IReadOnlyList<MediaPlaybackTimeRange>), typeof(MediaTimeline), new PropertyMetadata(Array.Empty<MediaPlaybackTimeRange>(), OnRangesChanged));
    public static readonly DependencyProperty DisabledRangesProperty = DependencyProperty.Register(
        nameof(DisabledRanges), typeof(IReadOnlyList<MediaPlaybackTimeRange>), typeof(MediaTimeline), new PropertyMetadata(Array.Empty<MediaPlaybackTimeRange>(), OnRangesChanged));
    public static readonly DependencyProperty ChaptersProperty = DependencyProperty.Register(
        nameof(Chapters), typeof(IReadOnlyList<MediaTimelineMarker>), typeof(MediaTimeline), new PropertyMetadata(Array.Empty<MediaTimelineMarker>(), OnTimelinePropertyChanged));
    public static readonly DependencyProperty MarkersProperty = DependencyProperty.Register(
        nameof(Markers), typeof(IReadOnlyList<MediaTimelineMarker>), typeof(MediaTimeline), new PropertyMetadata(Array.Empty<MediaTimelineMarker>(), OnTimelinePropertyChanged));
    public static readonly DependencyProperty IsSeekEnabledProperty = DependencyProperty.Register(
        nameof(IsSeekEnabled), typeof(bool), typeof(MediaTimeline), new PropertyMetadata(true, OnTimelinePropertyChanged));
    public static readonly DependencyProperty KeyboardStepProperty = DependencyProperty.Register(
        nameof(KeyboardStep), typeof(TimeSpan), typeof(MediaTimeline), new PropertyMetadata(TimeSpan.FromSeconds(5), OnTimelinePropertyChanged));
    public static readonly DependencyProperty LargeKeyboardStepProperty = DependencyProperty.Register(
        nameof(LargeKeyboardStep), typeof(TimeSpan), typeof(MediaTimeline), new PropertyMetadata(TimeSpan.FromSeconds(30), OnTimelinePropertyChanged));
    public static readonly DependencyProperty PreviewThrottleIntervalProperty = DependencyProperty.Register(
        nameof(PreviewThrottleInterval), typeof(TimeSpan), typeof(MediaTimeline), new PropertyMetadata(TimeSpan.FromMilliseconds(100), OnPreviewThrottleChanged));

    public MediaTimeline()
    {
        DefaultStyleKey = typeof(MediaTimeline);
        _previewTimer = DispatcherQueue.CreateTimer();
        _previewTimer.Interval = PreviewThrottleInterval;
        _previewTimer.Tick += OnPreviewTimerTick;
        Loaded += (_, _) => RefreshState();
        Unloaded += (_, _) => CancelScrub();
    }

    public MediaTimelineMode Mode { get => (MediaTimelineMode)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public TimeSpan Minimum { get => (TimeSpan)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public TimeSpan Maximum { get => (TimeSpan)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public TimeSpan Position { get => (TimeSpan)GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    public DateTimeOffset? LiveWindowEndTime { get => (DateTimeOffset?)GetValue(LiveWindowEndTimeProperty); set => SetValue(LiveWindowEndTimeProperty, value); }
    public TimeSpan LiveEdgeTolerance { get => (TimeSpan)GetValue(LiveEdgeToleranceProperty); set => SetValue(LiveEdgeToleranceProperty, value < TimeSpan.Zero ? TimeSpan.Zero : value); }
    public double PlaybackRate { get => (double)GetValue(PlaybackRateProperty); set => SetValue(PlaybackRateProperty, double.IsFinite(value) && value > 0 ? value : 1d); }
    public IReadOnlyList<MediaPlaybackTimeRange> BufferedRanges { get => (IReadOnlyList<MediaPlaybackTimeRange>)GetValue(BufferedRangesProperty); set => SetValue(BufferedRangesProperty, value ?? Array.Empty<MediaPlaybackTimeRange>()); }
    public IReadOnlyList<MediaPlaybackTimeRange> DisabledRanges { get => (IReadOnlyList<MediaPlaybackTimeRange>)GetValue(DisabledRangesProperty); set => SetValue(DisabledRangesProperty, value ?? Array.Empty<MediaPlaybackTimeRange>()); }
    public IReadOnlyList<MediaTimelineMarker> Chapters { get => (IReadOnlyList<MediaTimelineMarker>)GetValue(ChaptersProperty); set => SetValue(ChaptersProperty, value ?? Array.Empty<MediaTimelineMarker>()); }
    public IReadOnlyList<MediaTimelineMarker> Markers { get => (IReadOnlyList<MediaTimelineMarker>)GetValue(MarkersProperty); set => SetValue(MarkersProperty, value ?? Array.Empty<MediaTimelineMarker>()); }
    public bool IsSeekEnabled { get => (bool)GetValue(IsSeekEnabledProperty); set => SetValue(IsSeekEnabledProperty, value); }
    public TimeSpan KeyboardStep { get => (TimeSpan)GetValue(KeyboardStepProperty); set => SetValue(KeyboardStepProperty, value < TimeSpan.Zero ? TimeSpan.Zero : value); }
    public TimeSpan LargeKeyboardStep { get => (TimeSpan)GetValue(LargeKeyboardStepProperty); set => SetValue(LargeKeyboardStepProperty, value < TimeSpan.Zero ? TimeSpan.Zero : value); }
    public TimeSpan PreviewThrottleInterval { get => (TimeSpan)GetValue(PreviewThrottleIntervalProperty); set => SetValue(PreviewThrottleIntervalProperty, value <= TimeSpan.Zero ? TimeSpan.FromMilliseconds(100) : value); }

    public event EventHandler<MediaTimelinePreviewEventArgs>? PreviewPositionChanged;
    public event EventHandler<MediaTimelineSeekRequestedEventArgs>? SeekRequested;
    public event EventHandler? LiveEdgeRequested;

    /// <summary>Returns the currently normalized buffered snapshot for diagnostics and template authors.</summary>
    public IReadOnlyList<MediaPlaybackTimeRange> NormalizedBufferedRanges => _normalizedBufferedRanges;
    /// <summary>Returns the currently normalized disabled snapshot for diagnostics and template authors.</summary>
    public IReadOnlyList<MediaPlaybackTimeRange> NormalizedDisabledRanges => _normalizedDisabledRanges;
    public bool IsAtLiveEdge => Mode == MediaTimelineMode.LiveDvr && IsRangeValid && Position >= Maximum - ValidLiveEdgeTolerance;

    protected override AutomationPeer OnCreateAutomationPeer() => new MediaTimelineAutomationPeer(this);

    protected override void OnApplyTemplate()
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate();
        _root = GetTemplateChild("PART_RootGrid") as FrameworkElement
            ?? throw new InvalidOperationException("MediaTimeline requires a PART_RootGrid template part.");
        _track = GetTemplateChild("PART_Track") as Grid;
        _progress = GetTemplateChild("PART_Progress") as FrameworkElement;
        _bufferedPresenter = GetTemplateChild("PART_BufferedRangesPresenter") as Canvas;
        _disabledPresenter = GetTemplateChild("PART_DisabledRangesPresenter") as Canvas;
        _chapterPresenter = GetTemplateChild("PART_ChapterPresenter") as Canvas;
        _markerPresenter = GetTemplateChild("PART_MarkerPresenter") as Canvas;
        _thumb = GetTemplateChild("PART_Thumb") as Thumb;
        _goLiveButton = GetTemplateChild("PART_GoLiveButton") as ButtonBase;
        _timeText = GetTemplateChild("PART_TimeText") as TextBlock;
        _playbackRateText = GetTemplateChild("PART_PlaybackRateText") as TextBlock;
        _liveBadge = GetTemplateChild("PART_LiveBadge") as FrameworkElement;
        _root.SizeChanged += OnRootSizeChanged;
        if (_track is not null)
        {
            _track.SizeChanged += OnTrackSizeChanged;
            _track.PointerPressed += OnTrackPointerPressed;
        }
        if (_thumb is not null)
        {
            _thumb.PointerPressed += OnThumbPointerPressed;
            _thumb.DragStarted += OnThumbDragStarted;
            _thumb.DragDelta += OnThumbDragDelta;
            _thumb.DragCompleted += OnThumbDragCompleted;
        }
        if (_goLiveButton is not null)
        {
            _goLiveButton.Click += OnGoLiveClicked;
        }
        RefreshState();
    }

    private static void OnTimelinePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _) => ((MediaTimeline)sender).NormalizeRangesAndRefresh();
    private static void OnRangesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _) => ((MediaTimeline)sender).NormalizeRangesAndRefresh();
    private static void OnPreviewThrottleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var timeline = (MediaTimeline)sender;
        var interval = (TimeSpan)args.NewValue;
        if (interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(PreviewThrottleInterval));
        }
        timeline._previewTimer.Interval = interval;
    }

    private static void OnPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _) => ((MediaTimeline)sender).CoercePositionAndRefresh();

    private void NormalizeRangesAndRefresh()
    {
        NormalizeRanges(BufferedRanges, _normalizedBufferedRanges);
        NormalizeRanges(DisabledRanges, _normalizedDisabledRanges);
        CoercePositionAndRefresh();
    }

    private void CoercePositionAndRefresh()
    {
        if (_isCoercingPosition)
        {
            RefreshState();
            return;
        }
        var coerced = CoerceToValidPosition(Position, 0, Minimum, Maximum);
        if (coerced != Position)
        {
            _isCoercingPosition = true;
            SetValue(PositionProperty, coerced);
            _isCoercingPosition = false;
        }
        RefreshState();
    }

    private void NormalizeRanges(IReadOnlyList<MediaPlaybackTimeRange> source, List<MediaPlaybackTimeRange> target)
    {
        target.Clear();
        if (!IsRangeValid || source is null)
        {
            return;
        }
        target.AddRange(MediaTimelineMath.NormalizeRanges(source, Minimum, Maximum));
    }

    private bool IsRangeValid => Maximum >= Minimum;
    private TimeSpan ValidLiveEdgeTolerance => LiveEdgeTolerance < TimeSpan.Zero ? TimeSpan.Zero : LiveEdgeTolerance;
    private bool CanSeek => IsEnabled && IsSeekEnabled && IsRangeValid && Mode != MediaTimelineMode.Live;

    private TimeSpan CoerceToValidPosition(TimeSpan candidate, int direction, TimeSpan minimum, TimeSpan maximum)
    {
        if (maximum < minimum)
        {
            return minimum;
        }
        var value = candidate < minimum ? minimum : candidate > maximum ? maximum : candidate;
        foreach (var range in _normalizedDisabledRanges)
        {
            if (value < range.Start || value > range.End)
            {
                continue;
            }
            var backward = value - range.Start;
            var forward = range.End - value;
            return forward < backward || (forward == backward && direction >= 0) ? range.End : range.Start;
        }
        return value;
    }

    private void OnTrackPointerPressed(object sender, PointerRoutedEventArgs args)
    {
        if (!CanSeek || _track is null || _isScrubbing)
        {
            return;
        }
        var point = args.GetCurrentPoint(_track);
        var width = Math.Max(1d, _track.ActualWidth);
        var ratio = Math.Clamp(point.Position.X / width, 0d, 1d);
        if (FlowDirection == FlowDirection.RightToLeft)
        {
            ratio = 1d - ratio;
        }
        var target = Minimum + TimeSpan.FromTicks((long)((Maximum - Minimum).Ticks * ratio));
        SubmitSeek(target, GetInputKind(args), ratio >= PositionRatio ? 1 : -1);
        args.Handled = true;
    }

    private void OnThumbDragStarted(object sender, DragStartedEventArgs args)
    {
        if (!CanSeek)
        {
            return;
        }
        _isScrubbing = true;
        _scrubStartPosition = Position;
        _scrubMinimum = Minimum;
        _scrubMaximum = Maximum;
        _scrubDirection = 0;
        if (_scrubInputKind == MediaTimelineInputKind.Unknown)
        {
            _scrubInputKind = MediaTimelineInputKind.Mouse;
        }
        VisualStateManager.GoToState(this, "Scrubbing", true);
    }

    private void OnThumbDragDelta(object sender, DragDeltaEventArgs args)
    {
        if (!_isScrubbing || _track is null)
        {
            return;
        }
        var width = Math.Max(1d, _track.ActualWidth);
        var direction = args.HorizontalChange == 0 ? _scrubDirection : Math.Sign(args.HorizontalChange) * (FlowDirection == FlowDirection.RightToLeft ? -1 : 1);
        var scale = (_scrubMaximum - _scrubMinimum).Ticks / width;
        var target = Position + TimeSpan.FromTicks((long)(args.HorizontalChange * scale * (FlowDirection == FlowDirection.RightToLeft ? -1 : 1)));
        _scrubDirection = direction;
        Position = CoerceToValidPosition(target, direction, _scrubMinimum, _scrubMaximum);
        PublishPreview(Position, _scrubInputKind);
    }

    private void OnThumbDragCompleted(object sender, DragCompletedEventArgs args)
    {
        if (!_isScrubbing)
        {
            return;
        }
        if (args.Canceled)
        {
            CancelScrub();
            return;
        }
        _isScrubbing = false;
        FlushPreview();
        SubmitSeek(Position, _scrubInputKind, _scrubDirection);
    }

    private void PublishPreview(TimeSpan position, MediaTimelineInputKind inputKind)
    {
        _pendingPreview = position;
        _scrubInputKind = inputKind;
        if (_previewStopwatch.Elapsed >= PreviewThrottleInterval)
        {
            EmitPreview();
            return;
        }
        _hasPendingPreview = true;
        if (!_previewTimer.IsRunning)
        {
            _previewTimer.Start();
        }
    }

    private void OnPreviewTimerTick(DispatcherTimer sender, object args)
    {
        _previewTimer.Stop();
        if (_isScrubbing && _hasPendingPreview)
        {
            EmitPreview();
        }
    }

    private void EmitPreview()
    {
        _previewTimer.Stop();
        _hasPendingPreview = false;
        _previewStopwatch.Restart();
        PreviewPositionChanged?.Invoke(this, new MediaTimelinePreviewEventArgs(_pendingPreview, true, _scrubInputKind));
    }

    private void FlushPreview()
    {
        if (_hasPendingPreview)
        {
            EmitPreview();
        }
    }

    private void CancelScrub()
    {
        _previewTimer.Stop();
        _hasPendingPreview = false;
        if (_isScrubbing)
        {
            _isScrubbing = false;
            Position = _scrubStartPosition;
        }
        RefreshState();
    }

    private void SubmitSeek(TimeSpan target, MediaTimelineInputKind inputKind, int direction)
    {
        if (!CanSeek)
        {
            return;
        }
        var position = CoerceToValidPosition(target, direction, Minimum, Maximum);
        Position = position;
        SeekRequested?.Invoke(this, new MediaTimelineSeekRequestedEventArgs(position, true, inputKind));
        RaiseAutomationValueChanged();
    }

    internal void SetValueFromAutomation(double seconds)
    {
        if (double.IsNaN(seconds) || double.IsInfinity(seconds))
        {
            throw new ArgumentOutOfRangeException(nameof(seconds));
        }
        SubmitSeek(TimeSpan.FromSeconds(seconds), MediaTimelineInputKind.Unknown, seconds >= Position.TotalSeconds ? 1 : -1);
    }

    private void OnGoLiveClicked(object sender, RoutedEventArgs args) => RequestLiveEdge();

    private void OnThumbPointerPressed(object sender, PointerRoutedEventArgs args) =>
        _scrubInputKind = GetInputKind(args);

    private void RequestLiveEdge()
    {
        if (Mode != MediaTimelineMode.LiveDvr || !IsRangeValid)
        {
            return;
        }
        var wasHandled = LiveEdgeRequested is not null;
        LiveEdgeRequested?.Invoke(this, EventArgs.Empty);
        if (!wasHandled)
        {
            SubmitSeek(Maximum, MediaTimelineInputKind.Keyboard, 1);
        }
    }

    protected override void OnKeyDown(KeyRoutedEventArgs args)
    {
        base.OnKeyDown(args);
        if (!CanSeek)
        {
            return;
        }
        var visualForward = FlowDirection == FlowDirection.RightToLeft ? -1 : 1;
        var kind = args.Key.ToString().StartsWith("Gamepad", StringComparison.Ordinal) ? MediaTimelineInputKind.GameController : MediaTimelineInputKind.Keyboard;
        switch (args.Key)
        {
            case VirtualKey.Left:
            case VirtualKey.GamepadDPadLeft:
                SubmitSeek(Position - TimeSpan.FromTicks(KeyboardStep.Ticks * visualForward), kind, -visualForward);
                args.Handled = true;
                break;
            case VirtualKey.Right:
            case VirtualKey.GamepadDPadRight:
                SubmitSeek(Position + TimeSpan.FromTicks(KeyboardStep.Ticks * visualForward), kind, visualForward);
                args.Handled = true;
                break;
            case VirtualKey.PageUp:
                SubmitSeek(Position + LargeKeyboardStep, kind, 1);
                args.Handled = true;
                break;
            case VirtualKey.PageDown:
                SubmitSeek(Position - LargeKeyboardStep, kind, -1);
                args.Handled = true;
                break;
            case VirtualKey.Home:
                SubmitSeek(Minimum, kind, -1);
                args.Handled = true;
                break;
            case VirtualKey.End:
                if (Mode == MediaTimelineMode.LiveDvr)
                {
                    RequestLiveEdge();
                }
                else
                {
                    SubmitSeek(Maximum, kind, 1);
                }
                args.Handled = true;
                break;
        }
    }

    private double PositionRatio => !IsRangeValid || Maximum == Minimum ? 0d : (Position - Minimum).TotalMilliseconds / (Maximum - Minimum).TotalMilliseconds;

    private void RefreshState()
    {
        var state = !IsRangeValid ? "Unavailable" : !CanSeek ? "ReadOnly" : "Idle";
        VisualStateManager.GoToState(this, state, true);
        VisualStateManager.GoToState(this, Mode.ToString(), true);
        VisualStateManager.GoToState(this, IsAtLiveEdge ? "AtLiveEdge" : "NotAtLiveEdge", true);
        if (_timeText is not null)
        {
            _timeText.Text = Mode switch
            {
                MediaTimelineMode.Live => "Live",
                MediaTimelineMode.LiveDvr when LiveWindowEndTime is { } liveEnd
                    => (liveEnd - (Maximum - Position)).ToLocalTime().ToString("HH:mm:ss"),
                _ => Position.ToString("c")
            };
        }
        if (_playbackRateText is not null)
        {
            var rate = double.IsFinite(PlaybackRate) && PlaybackRate > 0 ? PlaybackRate : 1d;
            _playbackRateText.Text = rate == 1d ? string.Empty : $"{rate:0.##}x";
        }
        if (_liveBadge is not null)
        {
            _liveBadge.Visibility = Mode is MediaTimelineMode.Live or MediaTimelineMode.LiveDvr ? Visibility.Visible : Visibility.Collapsed;
        }
        if (_goLiveButton is not null)
        {
            _goLiveButton.Visibility = Mode == MediaTimelineMode.LiveDvr && !IsAtLiveEdge ? Visibility.Visible : Visibility.Collapsed;
        }
        UpdateTimelineVisuals();
        RaiseAutomationValueChanged();
    }

    private void UpdateTimelineVisuals()
    {
        if (_track is null || !IsRangeValid) return;
        var width = Math.Max(1, _track.ActualWidth);
        var range = (Maximum - Minimum).TotalMilliseconds;
        if (range <= 0)
        {
            if (_progress is not null) _progress.Width = 0;
            return;
        }
        var ratio = range <= 0 ? 0 : Math.Clamp((Position - Minimum).TotalMilliseconds / range, 0, 1);
        if (FlowDirection == FlowDirection.RightToLeft) ratio = 1 - ratio;
        if (_progress is not null) _progress.Width = width * ratio;
        if (_thumb is not null)
        {
            var thumbWidth = double.IsFinite(_thumb.ActualWidth) && _thumb.ActualWidth > 0 ? _thumb.ActualWidth : 16;
            _thumb.Margin = new Thickness(Math.Max(0, width * ratio - thumbWidth / 2), 0, 0, 0);
        }
        RenderRanges(_bufferedPresenter, _normalizedBufferedRanges, width, range, "MediaTimelineBufferedBrush");
        RenderRanges(_disabledPresenter, _normalizedDisabledRanges, width, range, "MediaTimelineDisabledBrush");
        RenderMarkers(_chapterPresenter, Chapters, width, range, "MediaTimelineChapterBrush");
        RenderMarkers(_markerPresenter, Markers, width, range, "MediaTimelineMarkerBrush");
    }

    private void OnRootSizeChanged(object sender, SizeChangedEventArgs args) => UpdateTimelineVisuals();
    private void OnTrackSizeChanged(object sender, SizeChangedEventArgs args) => UpdateTimelineVisuals();

    private void RenderRanges(Canvas? canvas, IReadOnlyList<MediaPlaybackTimeRange> ranges, double width, double totalMilliseconds, string brushKey)
    {
        if (canvas is null) return;
        canvas.Children.Clear();
        var brush = Application.Current.Resources.ContainsKey(brushKey) && Application.Current.Resources[brushKey] is Microsoft.UI.Xaml.Media.Brush typed ? typed : null;
        foreach (var range in ranges)
        {
            var start = Math.Clamp((range.Start - Minimum).TotalMilliseconds / totalMilliseconds, 0, 1);
            var end = Math.Clamp((range.End - Minimum).TotalMilliseconds / totalMilliseconds, 0, 1);
            if (FlowDirection == FlowDirection.RightToLeft) (start, end) = (1 - end, 1 - start);
            var rectangle = new Microsoft.UI.Xaml.Shapes.Rectangle { Width = Math.Max(1, (end - start) * width), Height = 4, Fill = brush, Opacity = .75 };
            Canvas.SetLeft(rectangle, start * width);
            canvas.Children.Add(rectangle);
        }
    }

    private void RenderMarkers(Canvas? canvas, IReadOnlyList<MediaTimelineMarker> markers, double width, double totalMilliseconds, string brushKey)
    {
        if (canvas is null) return;
        canvas.Children.Clear();
        var brush = Application.Current.Resources.ContainsKey(brushKey) && Application.Current.Resources[brushKey] is Microsoft.UI.Xaml.Media.Brush typed ? typed : null;
        foreach (var marker in markers)
        {
            var ratio = Math.Clamp((marker.Position - Minimum).TotalMilliseconds / totalMilliseconds, 0, 1);
            if (FlowDirection == FlowDirection.RightToLeft) ratio = 1 - ratio;
            var line = new Border { Width = 2, Height = 8, Background = brush, VerticalAlignment = VerticalAlignment.Center };
            Canvas.SetLeft(line, ratio * width);
            canvas.Children.Add(line);
        }
    }

    private void RaiseAutomationValueChanged()
    {
        if (FrameworkElementAutomationPeer.FromElement(this) is MediaTimelineAutomationPeer peer)
        {
            peer.RaiseValueChanged();
        }
    }

    private void DetachTemplateHandlers()
    {
        if (_root is not null) _root.SizeChanged -= OnRootSizeChanged;
        if (_track is not null) _track.SizeChanged -= OnTrackSizeChanged;
        if (_track is not null) _track.PointerPressed -= OnTrackPointerPressed;
        if (_thumb is not null)
        {
            _thumb.PointerPressed -= OnThumbPointerPressed;
            _thumb.DragStarted -= OnThumbDragStarted;
            _thumb.DragDelta -= OnThumbDragDelta;
            _thumb.DragCompleted -= OnThumbDragCompleted;
        }
        if (_goLiveButton is not null) _goLiveButton.Click -= OnGoLiveClicked;
        _root = null;
        _progress = null;
        _bufferedPresenter = null;
        _disabledPresenter = null;
        _chapterPresenter = null;
        _markerPresenter = null;
        _track = null;
        _thumb = null;
        _goLiveButton = null;
        _timeText = null;
        _playbackRateText = null;
        _liveBadge = null;
    }

    private static MediaTimelineInputKind GetInputKind(PointerRoutedEventArgs args) => args.Pointer.PointerDeviceType switch
    {
        Microsoft.UI.Input.PointerDeviceType.Mouse => MediaTimelineInputKind.Mouse,
        Microsoft.UI.Input.PointerDeviceType.Touch => MediaTimelineInputKind.Touch,
        Microsoft.UI.Input.PointerDeviceType.Pen => MediaTimelineInputKind.Pen,
        _ => MediaTimelineInputKind.Unknown,
    };

}
