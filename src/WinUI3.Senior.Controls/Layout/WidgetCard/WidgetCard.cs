using System.Windows.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Controls;

public enum WidgetCardSize { Small, Medium, Wide, Large }
public enum WidgetCardState { Expanded, Collapsed, Loading, Error }

[Flags]
public enum WidgetCardPauseReason
{
    None,
    NotVisible = 1,
    WindowInactive = 2,
    Host = 4,
}

public sealed record WidgetCardRefreshResult(object? Content = null, object? Header = null, DateTimeOffset? LastUpdated = null, string? ErrorMessage = null)
{
    public bool IsSuccess => string.IsNullOrWhiteSpace(ErrorMessage);
}

public sealed class WidgetCardRefreshFailedEventArgs(Exception error) : EventArgs
{
    public Exception Error { get; } = error ?? throw new ArgumentNullException(nameof(error));
}

/// <summary>Dashboard card with explicit loading/error state and host-owned refresh command.</summary>
public sealed class WidgetCard : ContentControl
{
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(WidgetCard), new PropertyMetadata(null));
    public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(nameof(Footer), typeof(object), typeof(WidgetCard), new PropertyMetadata(null));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(WidgetCard), new PropertyMetadata(true, OnExpandedChanged));
    public static readonly DependencyProperty IsCollapsibleProperty = DependencyProperty.Register(nameof(IsCollapsible), typeof(bool), typeof(WidgetCard), new PropertyMetadata(false));
    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof(Size), typeof(WidgetCardSize), typeof(WidgetCard), new PropertyMetadata(WidgetCardSize.Medium));
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(WidgetCardState), typeof(WidgetCard), new PropertyMetadata(WidgetCardState.Expanded, OnStateChanged));
    public static readonly DependencyProperty RefreshCommandProperty = DependencyProperty.Register(nameof(RefreshCommand), typeof(ICommand), typeof(WidgetCard), new PropertyMetadata(null));
    public static readonly DependencyProperty LastUpdatedProperty = DependencyProperty.Register(nameof(LastUpdated), typeof(DateTimeOffset?), typeof(WidgetCard), new PropertyMetadata(null));
    public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(nameof(ErrorMessage), typeof(string), typeof(WidgetCard), new PropertyMetadata(null));
    public static readonly DependencyProperty AutomationLabelProperty = DependencyProperty.Register(nameof(AutomationLabel), typeof(string), typeof(WidgetCard), new PropertyMetadata(null));
    public static readonly DependencyProperty IsHostVisibleProperty = DependencyProperty.Register(nameof(IsHostVisible), typeof(bool), typeof(WidgetCard), new PropertyMetadata(true, OnHostStateChanged));
    public static readonly DependencyProperty IsHostWindowActiveProperty = DependencyProperty.Register(nameof(IsHostWindowActive), typeof(bool), typeof(WidgetCard), new PropertyMetadata(true, OnHostStateChanged));
    public static readonly DependencyProperty IsAutoRefreshEnabledProperty = DependencyProperty.Register(nameof(IsAutoRefreshEnabled), typeof(bool), typeof(WidgetCard), new PropertyMetadata(false, OnRefreshPropertyChanged));
    public static readonly DependencyProperty RefreshIntervalProperty = DependencyProperty.Register(nameof(RefreshInterval), typeof(TimeSpan), typeof(WidgetCard), new PropertyMetadata(TimeSpan.FromMinutes(5), OnRefreshPropertyChanged));
    private readonly object _refreshGate = new();
    private CancellationTokenSource? _refreshCts;
    private DispatcherQueueTimer? _refreshTimer;
    private int _refreshRevision;
    private WidgetCardPauseReason _pauseReason;
    private Button? _refreshButton;
    private bool _disposed;
    private Func<CancellationToken, ValueTask<WidgetCardRefreshResult>>? _refreshProvider;

    public WidgetCard()
    {
        DefaultStyleKey = typeof(WidgetCard);
        IsTabStop = true;
        KeyDown += (_, e) => { if (e.Key == Windows.System.VirtualKey.Space && IsCollapsible) { ToggleExpanded(); e.Handled = true; } };
        Loaded += (_, _) => RestartAutoRefresh();
        Unloaded += (_, _) => StopAutoRefresh();
    }
    public object? Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public object? Footer { get => GetValue(FooterProperty); set => SetValue(FooterProperty, value); }
    public bool IsExpanded { get => (bool)GetValue(IsExpandedProperty); set => SetValue(IsExpandedProperty, value); }
    public bool IsCollapsible { get => (bool)GetValue(IsCollapsibleProperty); set => SetValue(IsCollapsibleProperty, value); }
    public WidgetCardSize Size { get => (WidgetCardSize)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }
    public WidgetCardState State { get => (WidgetCardState)GetValue(StateProperty); set => SetValue(StateProperty, value); }
    public ICommand? RefreshCommand { get => (ICommand?)GetValue(RefreshCommandProperty); set => SetValue(RefreshCommandProperty, value); }
    public DateTimeOffset? LastUpdated { get => (DateTimeOffset?)GetValue(LastUpdatedProperty); set => SetValue(LastUpdatedProperty, value); }
    public string? ErrorMessage { get => (string?)GetValue(ErrorMessageProperty); set => SetValue(ErrorMessageProperty, value); }
    public string? AutomationLabel { get => (string?)GetValue(AutomationLabelProperty); set => SetValue(AutomationLabelProperty, value); }
    public bool IsHostVisible { get => (bool)GetValue(IsHostVisibleProperty); set => SetValue(IsHostVisibleProperty, value); }
    public bool IsHostWindowActive { get => (bool)GetValue(IsHostWindowActiveProperty); set => SetValue(IsHostWindowActiveProperty, value); }
    public bool IsAutoRefreshEnabled { get => (bool)GetValue(IsAutoRefreshEnabledProperty); set => SetValue(IsAutoRefreshEnabledProperty, value); }
    public TimeSpan RefreshInterval { get => (TimeSpan)GetValue(RefreshIntervalProperty); set => SetValue(RefreshIntervalProperty, value <= TimeSpan.Zero ? TimeSpan.FromMinutes(5) : value); }
    public WidgetCardPauseReason PauseReason => _pauseReason;
    public Func<CancellationToken, ValueTask<WidgetCardRefreshResult>>? RefreshProvider
    {
        get => _refreshProvider;
        set
        {
            _refreshProvider = value;
            if (DispatcherQueue.HasThreadAccess) RestartAutoRefresh();
            else DispatcherQueue.TryEnqueue(RestartAutoRefresh);
        }
    }
    public event EventHandler? ExpandedChanged;
    public event EventHandler? RefreshRequested;
    public event EventHandler? RetryRequested;
    public event EventHandler? RefreshStarted;
    public event EventHandler? RefreshCompleted;
    public event EventHandler<WidgetCardRefreshFailedEventArgs>? RefreshFailed;
    public event EventHandler? PauseReasonChanged;

    public void ToggleExpanded()
    {
        if (!IsCollapsible || State is WidgetCardState.Loading) return;
        IsExpanded = !IsExpanded;
    }
    public bool RequestRefresh(object? parameter = null)
    {
        if (State == WidgetCardState.Loading) return false;
        if (RefreshCommand is { } command && command.CanExecute(parameter)) command.Execute(parameter);
        RefreshRequested?.Invoke(this, EventArgs.Empty);
        return true;
    }

    /// <summary>Runs the host-owned refresh provider with cancellation and stale-result protection.</summary>
    public async ValueTask<bool> RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed || RefreshProvider is null || PauseReason != WidgetCardPauseReason.None || State == WidgetCardState.Loading) return false;
        var revision = Interlocked.Increment(ref _refreshRevision);
        CancellationToken token;
        lock (_refreshGate)
        {
            _refreshCts?.Cancel();
            _refreshCts?.Dispose();
            _refreshCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            token = _refreshCts.Token;
        }
        var wasExpanded = IsExpanded;
        SetLoading(true);
        RefreshStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            var result = await RefreshProvider(token);
            token.ThrowIfCancellationRequested();
            if (_disposed || revision != _refreshRevision || PauseReason != WidgetCardPauseReason.None) return false;
            if (!result.IsSuccess)
            {
                SetError(result.ErrorMessage ?? "The widget refresh failed.");
                RefreshFailed?.Invoke(this, new WidgetCardRefreshFailedEventArgs(new InvalidOperationException(ErrorMessage)));
                return false;
            }
            if (result.Content is not null) Content = result.Content;
            if (result.Header is not null) Header = result.Header;
            LastUpdated = result.LastUpdated ?? DateTimeOffset.Now;
            IsExpanded = wasExpanded;
            SetLoading(false);
            RefreshCompleted?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { return false; }
        catch (Exception ex)
        {
            if (!_disposed && revision == _refreshRevision) { SetError(ex.Message); RefreshFailed?.Invoke(this, new WidgetCardRefreshFailedEventArgs(ex)); }
            return false;
        }
        finally
        {
            if (revision == _refreshRevision && State == WidgetCardState.Loading) SetLoading(false);
        }
    }

    public void SetPauseReason(WidgetCardPauseReason reason, bool enabled)
    {
        if (reason == WidgetCardPauseReason.None) return;
        var previous = _pauseReason;
        _pauseReason = enabled ? _pauseReason | reason : _pauseReason & ~reason;
        if (previous == _pauseReason) return;
        if (_pauseReason != WidgetCardPauseReason.None) StopAutoRefresh(); else RestartAutoRefresh();
        PauseReasonChanged?.Invoke(this, EventArgs.Empty);
    }
    public bool RequestRetry(object? parameter = null)
    {
        if (State != WidgetCardState.Error) return false;
        RetryRequested?.Invoke(this, EventArgs.Empty);
        return RequestRefresh(parameter);
    }
    public void SetLoading(bool isLoading) => State = isLoading ? WidgetCardState.Loading : (IsExpanded ? WidgetCardState.Expanded : WidgetCardState.Collapsed);
    public void SetError(string message) { ErrorMessage = message; State = WidgetCardState.Error; }
    public void SetUpdated(DateTimeOffset timestamp) { LastUpdated = timestamp; State = IsExpanded ? WidgetCardState.Expanded : WidgetCardState.Collapsed; }
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Interlocked.Increment(ref _refreshRevision);
        StopAutoRefresh();
        lock (_refreshGate) { _refreshCts?.Cancel(); _refreshCts?.Dispose(); _refreshCts = null; }
    }
    protected override void OnApplyTemplate()
    {
        if (_refreshButton is not null) _refreshButton.Click -= OnRefreshButtonClick;
        base.OnApplyTemplate();
        _refreshButton = GetTemplateChild("PART_RefreshButton") as Button;
        if (_refreshButton is not null) _refreshButton.Click += OnRefreshButtonClick;
        UpdateVisualState();
    }
    private static void OnExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { var card = (WidgetCard)d; if (card.State is not (WidgetCardState.Loading or WidgetCardState.Error)) card.State = (bool)e.NewValue ? WidgetCardState.Expanded : WidgetCardState.Collapsed; card.UpdateVisualState(); card.ExpandedChanged?.Invoke(card, EventArgs.Empty); }
    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WidgetCard)d).UpdateVisualState();
    private static void OnHostStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var card = (WidgetCard)d;
        card.SetPauseReason(WidgetCardPauseReason.NotVisible, !card.IsHostVisible);
        card.SetPauseReason(WidgetCardPauseReason.WindowInactive, !card.IsHostWindowActive);
    }
    private static void OnRefreshPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WidgetCard)d).RestartAutoRefresh();
    private void RestartAutoRefresh()
    {
        StopAutoRefresh();
        if (_disposed || !IsLoaded || !IsAutoRefreshEnabled || RefreshProvider is null || PauseReason != WidgetCardPauseReason.None) return;
        _refreshTimer ??= DispatcherQueue.GetForCurrentThread()?.CreateTimer();
        if (_refreshTimer is null) return;
        _refreshTimer.Interval = RefreshInterval < TimeSpan.FromSeconds(1) ? TimeSpan.FromSeconds(1) : RefreshInterval;
        _refreshTimer.IsRepeating = true;
        _refreshTimer.Tick -= OnRefreshTimerTick;
        _refreshTimer.Tick += OnRefreshTimerTick;
        _refreshTimer.Start();
    }
    private void StopAutoRefresh() => _refreshTimer?.Stop();
    private async void OnRefreshTimerTick(DispatcherQueueTimer sender, object args) => await RefreshAsync();
    private async void OnRefreshButtonClick(object sender, RoutedEventArgs e)
    {
        if (RefreshProvider is not null) await RefreshAsync(); else RequestRefresh();
    }
    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Collapsed", true);
        VisualStateManager.GoToState(this, State.ToString(), true);
    }
}
