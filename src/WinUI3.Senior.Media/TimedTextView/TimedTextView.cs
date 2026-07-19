using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;
using DispatcherQueueTimer = Microsoft.UI.Dispatching.DispatcherQueueTimer;

namespace WinUI3.Senior.Media;

/// <summary>
/// Displays immutable, provider-independent captions, lyrics, karaoke text, and bilingual timed text.
/// UI state is owned by its dispatcher and stale document revisions never replace the rendered snapshot.
/// </summary>
public sealed partial class TimedTextView : Control
{
    private readonly DispatcherQueueTimer _autoScrollResumeTimer;
    private TimedTextDocument? _acceptedDocument;
    private TimedTextTrack? _activeTrack;
    private TimedTextSegment? _activeSegment;
    private TimedTextProjection? _currentProjection;
    private string? _activeSegmentId;
    private ItemsRepeater? _itemsRepeater;
    private ScrollViewer? _scrollViewer;
    private TextBlock? _singleLinePresenter;
    private TextBlock? _karaokePresenter;
    private TextBlock? _primaryTextPresenter;
    private TextBlock? _translationTextPresenter;
    private FrameworkElement? _emptyPresenter;
    private TextBlock? _liveRegion;
    private IReadOnlyList<TimedTextSegmentItem> _segmentItems = Array.Empty<TimedTextSegmentItem>();
    private TimedTextTrack? _renderedTrack;
    private int _lastActiveSegmentIndex = -1;
    private bool _isAutoScrollRequestPending;
    private bool _isAutoScrollSuspended;
    private bool _isUnloaded;

    public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
        nameof(Document), typeof(TimedTextDocument), typeof(TimedTextView), new PropertyMetadata(null, OnTimedTextPropertyChanged));

    public static readonly DependencyProperty ActiveTrackIdProperty = DependencyProperty.Register(
        nameof(ActiveTrackId), typeof(string), typeof(TimedTextView), new PropertyMetadata(null, OnTimedTextPropertyChanged));

    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        nameof(Position), typeof(TimeSpan), typeof(TimedTextView), new PropertyMetadata(TimeSpan.Zero, OnTimedTextPropertyChanged));

    public static readonly DependencyProperty TimingOffsetProperty = DependencyProperty.Register(
        nameof(TimingOffset), typeof(TimeSpan), typeof(TimedTextView), new PropertyMetadata(TimeSpan.Zero, OnTimedTextPropertyChanged));

    public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(
        nameof(DisplayMode), typeof(TimedTextDisplayMode), typeof(TimedTextView), new PropertyMetadata(TimedTextDisplayMode.SingleLine, OnTimedTextPropertyChanged));

    public static readonly DependencyProperty IsAutoScrollEnabledProperty = DependencyProperty.Register(
        nameof(IsAutoScrollEnabled), typeof(bool), typeof(TimedTextView), new PropertyMetadata(true, OnTimedTextPropertyChanged));

    public static readonly DependencyProperty ContextLineCountProperty = DependencyProperty.Register(
        nameof(ContextLineCount), typeof(int), typeof(TimedTextView), new PropertyMetadata(2, OnContextLineCountChanged));

    public TimedTextView()
    {
        DefaultStyleKey = typeof(TimedTextView);
        _autoScrollResumeTimer = DispatcherQueue.CreateTimer();
        _autoScrollResumeTimer.Interval = TimeSpan.FromSeconds(5);
        _autoScrollResumeTimer.Tick += OnAutoScrollResumeTimerTick;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        KeyDown += OnKeyDown;
    }

    public TimedTextDocument? Document
    {
        get => (TimedTextDocument?)GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    public string? ActiveTrackId
    {
        get => (string?)GetValue(ActiveTrackIdProperty);
        set => SetValue(ActiveTrackIdProperty, value);
    }

    public TimeSpan Position
    {
        get => (TimeSpan)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public TimeSpan TimingOffset
    {
        get => (TimeSpan)GetValue(TimingOffsetProperty);
        set => SetValue(TimingOffsetProperty, value);
    }

    public TimedTextDisplayMode DisplayMode
    {
        get => (TimedTextDisplayMode)GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    public bool IsAutoScrollEnabled
    {
        get => (bool)GetValue(IsAutoScrollEnabledProperty);
        set => SetValue(IsAutoScrollEnabledProperty, value);
    }

    public int ContextLineCount
    {
        get => (int)GetValue(ContextLineCountProperty);
        set => SetValue(ContextLineCountProperty, value);
    }

    public event EventHandler<TimedTextActiveSegmentChangedEventArgs>? ActiveSegmentChanged;
    public event EventHandler<TimedTextSegmentInvokedEventArgs>? SegmentInvoked;
    public event EventHandler<TimedTextTrackChangedEventArgs>? ActiveTrackChanged;

    /// <summary>The accepted revision currently rendered by this view, or null when empty.</summary>
    public TimedTextDocument? AcceptedDocument => _acceptedDocument;

    /// <summary>The track selected after language/role fallback.</summary>
    public TimedTextTrack? ActiveTrack => _activeTrack;

    /// <summary>The half-open interval containing the current effective position.</summary>
    public TimedTextSegment? ActiveSegment => _activeSegment;

    /// <summary>Immutable render projection for the current position and selected track.</summary>
    public TimedTextProjection? CurrentProjection => _currentProjection;

    /// <summary>Projects an arbitrary playback position without mutating this view.</summary>
    public TimedTextProjection? GetProjection(TimeSpan position) =>
        TimedTextProjectionCalculator.Project(_acceptedDocument, ActiveTrackId, position, TimingOffset, ContextLineCount);

    public bool IsAutoScrollSuspended => _isAutoScrollSuspended;

    /// <summary>Explicitly selects a track, preserving the host-owned document.</summary>
    public bool SelectTrack(string trackId)
    {
        if (_acceptedDocument is null || string.IsNullOrWhiteSpace(trackId) ||
            !_acceptedDocument.Tracks.Any(track => StringComparer.Ordinal.Equals(track.Id, trackId)))
        {
            return false;
        }
        ActiveTrackId = trackId;
        return true;
    }

    public bool SelectNextTrack(bool reverse = false)
    {
        if (_acceptedDocument is null || _acceptedDocument.Tracks.Count == 0) return false;
        var current = _activeTrack is null ? -1 : _acceptedDocument.Tracks.ToList().FindIndex(track => ReferenceEquals(track, _activeTrack));
        var next = current < 0 ? 0 : (current + (reverse ? -1 : 1) + _acceptedDocument.Tracks.Count) % _acceptedDocument.Tracks.Count;
        return SelectTrack(_acceptedDocument.Tracks[next].Id);
    }

    public void SuspendAutoScroll(TimeSpan? resumeAfter = null)
    {
        _isAutoScrollSuspended = true;
        _autoScrollResumeTimer.Stop();
        if (resumeAfter is { } interval && interval > TimeSpan.Zero)
        {
            _autoScrollResumeTimer.Interval = interval;
            _autoScrollResumeTimer.Start();
        }
        UpdateVisualStates();
    }

    public void ResumeAutoScroll()
    {
        _autoScrollResumeTimer.Stop();
        _isAutoScrollSuspended = false;
        UpdateVisualStates();
        RequestAutoScroll();
    }

    public TimedTextNavigator? CreateNavigator() => _acceptedDocument is null ? null : new TimedTextNavigator(_acceptedDocument);

    /// <summary>Moves the playback position to a segment and emits a user-style invocation request.</summary>
    public bool TryNavigateToSegment(string segmentId, bool isUserInitiated = true)
    {
        if (_acceptedDocument is null || _activeTrack is null || string.IsNullOrWhiteSpace(segmentId)) return false;
        var segment = _activeTrack.Segments.FirstOrDefault(item => StringComparer.Ordinal.Equals(item.Id, segmentId));
        if (segment is null) return false;

        Position = SubtractWithSaturation(segment.Start, TimingOffset);
        if (isUserInitiated)
        {
            SegmentInvoked?.Invoke(this, new TimedTextSegmentInvokedEventArgs(
                _acceptedDocument, _activeTrack, segment, segment.Start, true));
        }
        return true;
    }

    public bool TryNavigateNext(bool isUserInitiated = true)
    {
        if (_activeTrack is null) return false;
        var next = _activeSegment is null
            ? _activeTrack.Segments.FirstOrDefault()
            : _activeTrack.Segments.FirstOrDefault(segment => segment.Start > _activeSegment.Start);
        return next is not null && TryNavigateToSegment(next.Id, isUserInitiated);
    }

    public bool TryNavigatePrevious(bool isUserInitiated = true)
    {
        if (_activeTrack is null) return false;
        var previous = _activeSegment is null
            ? _activeTrack.Segments.LastOrDefault()
            : _activeTrack.Segments.Where(segment => segment.End <= _activeSegment.Start).LastOrDefault();
        return previous is not null && TryNavigateToSegment(previous.Id, isUserInitiated);
    }

    protected override void OnApplyTemplate()
    {
        if (_itemsRepeater is not null)
        {
            _itemsRepeater.PointerPressed -= OnRepeaterPointerPressed;
            _itemsRepeater.ElementPrepared -= OnRepeaterElementPrepared;
        }

        if (_scrollViewer is not null)
        {
            _scrollViewer.ViewChanged -= OnScrollViewerViewChanged;
        }

        base.OnApplyTemplate();
        _itemsRepeater = GetTemplateChild("PART_ItemsRepeater") as ItemsRepeater;
        _renderedTrack = null;
        _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
        _singleLinePresenter = GetTemplateChild("PART_SingleLinePresenter") as TextBlock;
        _karaokePresenter = GetTemplateChild("PART_KaraokePresenter") as TextBlock;
        _primaryTextPresenter = GetTemplateChild("PART_PrimaryTextPresenter") as TextBlock;
        _translationTextPresenter = GetTemplateChild("PART_TranslationTextPresenter") as TextBlock;
        _emptyPresenter = GetTemplateChild("PART_EmptyPresenter") as FrameworkElement;
        _liveRegion = GetTemplateChild("PART_LiveRegion") as TextBlock;

        if (_itemsRepeater is not null)
        {
            _itemsRepeater.PointerPressed += OnRepeaterPointerPressed;
            _itemsRepeater.ElementPrepared += OnRepeaterElementPrepared;
        }

        if (_scrollViewer is not null)
        {
            _scrollViewer.ViewChanged += OnScrollViewerViewChanged;
        }

        RefreshVisualState(false);
    }

    private static void OnTimedTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((TimedTextView)sender).ApplyPropertyChange(args.Property == DocumentProperty);

    private static void OnContextLineCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var view = (TimedTextView)sender;
        var value = (int)args.NewValue;
        if (value is < 0 or > 10)
        {
            view.SetValue(ContextLineCountProperty, Math.Clamp(value, 0, 10));
            return;
        }

        view.ApplyPropertyChange(false);
    }

    private void ApplyPropertyChange(bool documentChanged)
    {
        if (_isUnloaded)
        {
            return;
        }

        if (documentChanged)
        {
            var candidate = TimedTextNormalizer.Normalize(Document);
            if (candidate is null)
            {
                _acceptedDocument = null;
            }
            else if (_acceptedDocument is null || !StringComparer.Ordinal.Equals(candidate.Id, _acceptedDocument.Id) || candidate.Revision > _acceptedDocument.Revision)
            {
                _acceptedDocument = candidate;
            }
        }

        RefreshVisualState(true);
    }

    private void RefreshVisualState(bool announceActiveChange)
    {
        var previousTrack = _activeTrack;
        _activeTrack = SelectTrack(_acceptedDocument, ActiveTrackId);
        var queryPosition = AddWithSaturation(Position, TimingOffset);
        _currentProjection = TimedTextProjectionCalculator.Project(
            _acceptedDocument,
            _activeTrack?.Id,
            Position,
            TimingOffset,
            ContextLineCount);
        var nextSegment = _activeTrack is null ? null : FindActiveSegment(_activeTrack.Segments, queryPosition);
        var changed = !StringComparer.Ordinal.Equals(_activeSegmentId, nextSegment?.Id);
        _activeSegment = nextSegment;
        _activeSegmentId = nextSegment?.Id;

        if (!ReferenceEquals(previousTrack, _activeTrack))
        {
            ActiveTrackChanged?.Invoke(this, new TimedTextTrackChangedEventArgs(previousTrack, _activeTrack));
        }

        UpdateTemplateContent(queryPosition);
        UpdateVisualStates();

        if (changed && _acceptedDocument is not null && _activeTrack is not null)
        {
            if (announceActiveChange && _liveRegion is not null && nextSegment is not null)
            {
                _liveRegion.Text = nextSegment.Text;
                AutomationProperties.SetLiveSetting(_liveRegion, Microsoft.UI.Xaml.Automation.Peers.AutomationLiveSetting.Polite);
            }

            ActiveSegmentChanged?.Invoke(this, new TimedTextActiveSegmentChangedEventArgs(
                _acceptedDocument, _activeTrack, nextSegment, queryPosition, false));
            RequestAutoScroll();
        }
    }

    private void UpdateTemplateContent(TimeSpan queryPosition)
    {
        var activeText = _activeSegment?.Text ?? string.Empty;
        var translation = GetTranslation(_activeSegment, queryPosition);
        if (_singleLinePresenter is not null)
        {
            _singleLinePresenter.Text = activeText;
        }

        if (_karaokePresenter is not null)
        {
            _karaokePresenter.Text = activeText;
        }

        if (_primaryTextPresenter is not null)
        {
            _primaryTextPresenter.Text = activeText;
        }

        if (_translationTextPresenter is not null)
        {
            _translationTextPresenter.Text = translation ?? string.Empty;
            _translationTextPresenter.Visibility = string.IsNullOrWhiteSpace(translation) ? Visibility.Collapsed : Visibility.Visible;
        }

        if (_emptyPresenter is not null)
        {
            _emptyPresenter.Visibility = _activeTrack is null ? Visibility.Visible : Visibility.Collapsed;
        }

        if (_itemsRepeater is not null && !ReferenceEquals(_renderedTrack, _activeTrack))
        {
            _renderedTrack = _activeTrack;
            _segmentItems = _activeTrack?.Segments
                .Select(segment => new TimedTextSegmentItem(segment, StringComparer.Ordinal.Equals(segment.Id, _activeSegmentId)))
                .ToArray() ?? Array.Empty<TimedTextSegmentItem>();
            _lastActiveSegmentIndex = FindSegmentIndex(_activeSegmentId);
            _itemsRepeater.ItemsSource = _segmentItems;
        }

        UpdateRealizedSegmentHighlights();
    }

    private void UpdateVisualStates()
    {
        _ = VisualStateManager.GoToState(this, _activeTrack is null ? "Empty" : "Ready", false);
        _ = VisualStateManager.GoToState(this, _isAutoScrollSuspended ? "AutoScrollSuspended" : "AutoScrollEnabled", false);
        _ = VisualStateManager.GoToState(this, DisplayMode switch
        {
            TimedTextDisplayMode.ScrollingLyrics => "ScrollingLyrics",
            TimedTextDisplayMode.Karaoke => "Karaoke",
            TimedTextDisplayMode.Bilingual => "Bilingual",
            _ => "SingleLine",
        }, false);
    }

    private void RequestAutoScroll()
    {
        if (!IsAutoScrollEnabled || _isAutoScrollSuspended || _itemsRepeater is null || _activeSegmentId is null)
        {
            return;
        }

        var index = _activeTrack?.Segments.ToList().FindIndex(segment => StringComparer.Ordinal.Equals(segment.Id, _activeSegmentId)) ?? -1;
        if (index >= 0 && _itemsRepeater.TryGetElement(index) is FrameworkElement element)
        {
            _isAutoScrollRequestPending = true;
            element.StartBringIntoView();
            _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () => _isAutoScrollRequestPending = false);
        }
    }

    private void OnScrollViewerViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        if (!args.IsIntermediate || _isAutoScrollRequestPending)
        {
            return;
        }

        SuspendAutoScroll(TimeSpan.FromSeconds(5));
    }

    private void OnAutoScrollResumeTimerTick(DispatcherQueueTimer sender, object args)
    {
        sender.Stop();
        ResumeAutoScroll();
    }

    private void OnRepeaterPointerPressed(object sender, PointerRoutedEventArgs args)
    {
        if (_acceptedDocument is null || _activeTrack is null)
        {
            return;
        }

        var source = args.OriginalSource as FrameworkElement;
        var item = source?.DataContext as TimedTextSegmentItem;
        if (item is null)
        {
            return;
        }

        SegmentInvoked?.Invoke(this, new TimedTextSegmentInvokedEventArgs(
            _acceptedDocument, _activeTrack, item.Segment, item.Segment.Start, true));
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs args)
    {
        if (args.Key is not (VirtualKey.Enter or VirtualKey.Space) || _acceptedDocument is null || _activeTrack is null || _activeSegment is null)
        {
            return;
        }

        args.Handled = true;
        SegmentInvoked?.Invoke(this, new TimedTextSegmentInvokedEventArgs(
            _acceptedDocument, _activeTrack, _activeSegment, AddWithSaturation(Position, TimingOffset), true));
    }

    private void OnRepeaterElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        ApplySegmentAppearance(args.Element as TextBlock, (args.Element as FrameworkElement)?.DataContext as TimedTextSegmentItem);
    }

    private void UpdateRealizedSegmentHighlights()
    {
        var nextIndex = FindSegmentIndex(_activeSegmentId);
        if (_lastActiveSegmentIndex >= 0 && _lastActiveSegmentIndex != nextIndex)
        {
            _segmentItems[_lastActiveSegmentIndex].IsActive = false;
            ApplySegmentAppearance(_itemsRepeater?.TryGetElement(_lastActiveSegmentIndex) as TextBlock, _segmentItems[_lastActiveSegmentIndex]);
        }

        if (nextIndex >= 0)
        {
            _segmentItems[nextIndex].IsActive = true;
            ApplySegmentAppearance(_itemsRepeater?.TryGetElement(nextIndex) as TextBlock, _segmentItems[nextIndex]);
        }

        _lastActiveSegmentIndex = nextIndex;
    }

    private static void ApplySegmentAppearance(TextBlock? textBlock, TimedTextSegmentItem? item)
    {
        if (textBlock is not null && item is not null)
        {
            var key = item.IsActive ? "TimedTextHighlightBrush" : "TimedTextPrimaryForegroundBrush";
            if (Application.Current.Resources.ContainsKey(key) && Application.Current.Resources[key] is Brush brush)
            {
                textBlock.Foreground = brush;
            }
        }
    }

    private int FindSegmentIndex(string? segmentId)
    {
        if (segmentId is null)
        {
            return -1;
        }

        for (var index = 0; index < _segmentItems.Count; index++)
        {
            if (StringComparer.Ordinal.Equals(_segmentItems[index].Segment.Id, segmentId))
            {
                return index;
            }
        }

        return -1;
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        _isUnloaded = false;
        if (_scrollViewer is not null)
        {
            _scrollViewer.ViewChanged -= OnScrollViewerViewChanged;
            _scrollViewer.ViewChanged += OnScrollViewerViewChanged;
        }
        // A host may replace the document while this page is detached. Accept
        // that snapshot only after the view is live again, preserving revision fencing.
        ApplyPropertyChange(documentChanged: true);
    }

    private void OnUnloaded(object sender, RoutedEventArgs args)
    {
        _isUnloaded = true;
        _autoScrollResumeTimer.Stop();
        if (_scrollViewer is not null)
        {
            _scrollViewer.ViewChanged -= OnScrollViewerViewChanged;
        }
    }

    private TimedTextTrack? SelectTrack(TimedTextDocument? document, string? requestedTrackId)
    {
        if (document is null)
        {
            return null;
        }

        if (document.Tracks.Count == 0)
        {
            return null;
        }

        var explicitTrack = document.Tracks.FirstOrDefault(track => StringComparer.Ordinal.Equals(track.Id, requestedTrackId));
        if (explicitTrack is not null)
        {
            return explicitTrack;
        }

        var culture = System.Globalization.CultureInfo.CurrentUICulture;
        return document.Tracks.FirstOrDefault(track => !string.IsNullOrWhiteSpace(track.Language) &&
                                                       (culture.Name.StartsWith(track.Language, StringComparison.OrdinalIgnoreCase) ||
                                                        track.Language.StartsWith(culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase)))
            ?? document.Tracks[0];
    }

    private static TimedTextSegment? FindActiveSegment(IReadOnlyList<TimedTextSegment> segments, TimeSpan position) =>
        segments.Where(segment => segment.Start <= position && position < segment.End)
            .OrderByDescending(segment => segment.Start)
            .ThenBy(segment => segment.End)
            .FirstOrDefault();

    private string? GetTranslation(TimedTextSegment? segment, TimeSpan position)
    {
        if (segment is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(segment.TranslatedText))
        {
            return segment.TranslatedText;
        }

        var translationTrack = _acceptedDocument?.Tracks.FirstOrDefault(track => track.Role == TimedTextTrackRole.Translation);
        return translationTrack?.Segments.FirstOrDefault(candidate =>
            candidate.Start < segment.End && candidate.End > segment.Start &&
            candidate.Start <= position && position < candidate.End)?.Text;
    }

    private static TimeSpan AddWithSaturation(TimeSpan position, TimeSpan offset)
    {
        try
        {
            return position + offset;
        }
        catch (OverflowException)
        {
            return offset >= TimeSpan.Zero ? TimeSpan.MaxValue : TimeSpan.MinValue;
        }
    }

    private static TimeSpan SubtractWithSaturation(TimeSpan position, TimeSpan offset)
    {
        try
        {
            return position - offset;
        }
        catch (OverflowException)
        {
            return offset >= TimeSpan.Zero ? TimeSpan.MinValue : TimeSpan.MaxValue;
        }
    }
}
