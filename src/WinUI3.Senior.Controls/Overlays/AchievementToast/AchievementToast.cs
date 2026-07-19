using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Controls;

public enum AchievementRarity
{
    Common,
    Rare,
    Epic,
    Legendary,
}

public enum AchievementToastState
{
    Queued,
    Showing,
    Visible,
    Closing,
    HostDestroyed,
}

public sealed record AchievementToastRequest(
    string Id,
    string Title,
    string? Description = null,
    object? Icon = null,
    double Progress = 1,
    AchievementRarity Rarity = AchievementRarity.Common,
    TimeSpan? Duration = null,
    object? Tag = null) : EventArgs
{
    public AchievementToastRequest Normalize()
    {
        if (string.IsNullOrWhiteSpace(Id)) throw new ArgumentException("An achievement id is required.", nameof(Id));
        if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("An achievement title is required.", nameof(Title));
        var duration = Duration ?? TimeSpan.FromSeconds(6);
        if (duration < TimeSpan.FromMilliseconds(500)) duration = TimeSpan.FromMilliseconds(500);
        if (duration > TimeSpan.FromMinutes(10)) duration = TimeSpan.FromMinutes(10);
        return this with { Progress = double.IsFinite(Progress) ? Math.Clamp(Progress, 0, 1) : 0, Duration = duration };
    }
}

public sealed class AchievementToastEventArgs(AchievementToastRequest request, AchievementToastState state) : EventArgs
{
    public AchievementToastRequest Request { get; } = request ?? throw new ArgumentNullException(nameof(request));
    public AchievementToastState State { get; } = state;
}

public sealed class AchievementToastResult(AchievementToastState state, string? errorCode = null)
{
    public AchievementToastState State { get; } = state;
    public string? ErrorCode { get; } = errorCode;
    public bool IsShown => State is AchievementToastState.Visible or AchievementToastState.Closing;
    public bool IsHostDestroyed => State == AchievementToastState.HostDestroyed;
}

[TemplatePart(Name = "PART_Root", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_Icon", Type = typeof(ContentPresenter))]
[TemplatePart(Name = "PART_Title", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_Description", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_Progress", Type = typeof(ProgressBar))]
public sealed class AchievementToast : Control, IDisposable
{
    private readonly Queue<(AchievementToastRequest Request, TaskCompletionSource<AchievementToastResult> Completion)> _queue = new();
    private DispatcherQueueTimer? _timer;
    private FrameworkElement? _root;
    private ContentPresenter? _icon;
    private TextBlock? _title;
    private TextBlock? _description;
    private ProgressBar? _progress;
    private bool _disposed;

    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(AchievementToast), new PropertyMetadata(false));
    public static readonly DependencyProperty CurrentRequestProperty = DependencyProperty.Register(nameof(CurrentRequest), typeof(AchievementToastRequest), typeof(AchievementToast), new PropertyMetadata(null));
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(AchievementToastState), typeof(AchievementToast), new PropertyMetadata(AchievementToastState.Queued));
    public static readonly DependencyProperty MaximumQueueLengthProperty = DependencyProperty.Register(nameof(MaximumQueueLength), typeof(int), typeof(AchievementToast), new PropertyMetadata(32));

    public AchievementToast()
    {
        DefaultStyleKey = typeof(AchievementToast);
        Loaded += OnLoaded;
        Unloaded += (_, _) => StopTimer();
    }

    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); private set => SetValue(IsOpenProperty, value); }
    public AchievementToastRequest? CurrentRequest { get => (AchievementToastRequest?)GetValue(CurrentRequestProperty); private set => SetValue(CurrentRequestProperty, value); }
    public AchievementToastState State { get => (AchievementToastState)GetValue(StateProperty); private set => SetValue(StateProperty, value); }
    public int MaximumQueueLength { get => (int)GetValue(MaximumQueueLengthProperty); set => SetValue(MaximumQueueLengthProperty, Math.Clamp(value, 1, 256)); }

    public event EventHandler<AchievementToastEventArgs>? StateChanged;
    public event EventHandler<AchievementToastRequest>? ActionRequested;

    public Task<AchievementToastResult> ShowAsync(AchievementToastRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (_disposed) return Task.FromResult(new AchievementToastResult(AchievementToastState.HostDestroyed, "host-destroyed"));
        var normalized = request.Normalize();
        if (cancellationToken.IsCancellationRequested) return Task.FromCanceled<AchievementToastResult>(cancellationToken);
        var completion = new TaskCompletionSource<AchievementToastResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (CurrentRequest?.Id == normalized.Id) return Task.FromResult(new AchievementToastResult(AchievementToastState.Visible));
        if (_queue.Count >= MaximumQueueLength)
        {
            completion.TrySetResult(new AchievementToastResult(AchievementToastState.Queued, "queue-full"));
            return completion.Task;
        }
        _queue.Enqueue((normalized, completion));
        SetState(AchievementToastState.Queued, normalized);
        TryShowNext();
        cancellationToken.Register(() =>
        {
            void CancelOnOwnerQueue() => CancelPending(normalized.Id, completion, cancellationToken);
            if (DispatcherQueue is { } queue) queue.TryEnqueue(CancelOnOwnerQueue);
            else CancelOnOwnerQueue();
        });
        return completion.Task;
    }

    public bool Dismiss()
    {
        if (!IsOpen || CurrentRequest is null) return false;
        SetState(AchievementToastState.Closing, CurrentRequest);
        CompleteCurrent(new AchievementToastResult(AchievementToastState.Closing));
        return true;
    }

    public bool InvokeAction()
    {
        if (CurrentRequest is not { } request) return false;
        ActionRequested?.Invoke(this, request);
        return true;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        StopTimer();
        while (_queue.Count > 0) _queue.Dequeue().Completion.TrySetResult(new AchievementToastResult(AchievementToastState.HostDestroyed, "host-destroyed"));
        CompleteCurrent(new AchievementToastResult(AchievementToastState.HostDestroyed, "host-destroyed"));
        CurrentRequest = null;
        IsOpen = false;
        SetValue(StateProperty, AchievementToastState.HostDestroyed);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _root = GetTemplateChild("PART_Root") as FrameworkElement;
        _icon = GetTemplateChild("PART_Icon") as ContentPresenter;
        _title = GetTemplateChild("PART_Title") as TextBlock;
        _description = GetTemplateChild("PART_Description") as TextBlock;
        _progress = GetTemplateChild("PART_Progress") as ProgressBar;
        UpdateTemplate();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    private TaskCompletionSource<AchievementToastResult>? _currentCompletion;
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        EnsureTimer();
        if (CurrentRequest is not null && !_disposed && _timer is not null)
        {
            _timer.IsRepeating = false;
            _timer.Interval = _currentRequestDuration();
            _timer.Tick -= OnTimerTick;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }
    }
    private TimeSpan _currentRequestDuration() => CurrentRequest?.Duration ?? TimeSpan.FromSeconds(6);
    private void EnsureTimer() => _timer ??= DispatcherQueue.GetForCurrentThread()?.CreateTimer();
    private void StopTimer() => _timer?.Stop();
    private void TryShowNext()
    {
        if (_disposed || IsOpen || _queue.Count == 0) return;
        EnsureTimer();
        var next = _queue.Dequeue();
        _currentCompletion = next.Completion;
        CurrentRequest = next.Request;
        IsOpen = true;
        SetState(AchievementToastState.Showing, next.Request);
        UpdateTemplate();
        SetState(AchievementToastState.Visible, next.Request);
        if (_timer is not null)
        {
            _timer.IsRepeating = false;
            _timer.Interval = next.Request.Duration ?? TimeSpan.FromSeconds(6);
            _timer.Tick -= OnTimerTick;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }
    }

    private void OnTimerTick(DispatcherQueueTimer sender, object args) => Dismiss();
    private void CancelPending(string id, TaskCompletionSource<AchievementToastResult> completion, CancellationToken token)
    {
        if (completion.Task.IsCompleted) return;
        if (ReferenceEquals(_currentCompletion, completion))
        {
            completion.TrySetCanceled(token);
            CompleteCurrent(new AchievementToastResult(AchievementToastState.Queued, "cancelled"));
            return;
        }

        if (_queue.Count == 0) return;
        var retained = new Queue<(AchievementToastRequest Request, TaskCompletionSource<AchievementToastResult> Completion)>();
        while (_queue.Count > 0)
        {
            var entry = _queue.Dequeue();
            if (string.Equals(entry.Request.Id, id, StringComparison.Ordinal) && ReferenceEquals(entry.Completion, completion))
            {
                completion.TrySetCanceled(token);
            }
            else retained.Enqueue(entry);
        }
        while (retained.Count > 0) _queue.Enqueue(retained.Dequeue());
    }
    private void CompleteCurrent(AchievementToastResult result)
    {
        StopTimer();
        _currentCompletion?.TrySetResult(result);
        _currentCompletion = null;
        IsOpen = false;
        CurrentRequest = null;
        UpdateTemplate();
        TryShowNext();
    }

    private void SetState(AchievementToastState state, AchievementToastRequest? request)
    {
        State = state;
        if (request is not null) StateChanged?.Invoke(this, new AchievementToastEventArgs(request, state));
    }

    private void UpdateTemplate()
    {
        if (_root is not null) _root.Visibility = IsOpen ? Visibility.Visible : Visibility.Collapsed;
        if (CurrentRequest is not { } request) return;
        if (_icon is not null) _icon.Content = request.Icon;
        if (_title is not null) _title.Text = request.Title;
        if (_description is not null) { _description.Text = request.Description ?? string.Empty; _description.Visibility = string.IsNullOrWhiteSpace(request.Description) ? Visibility.Collapsed : Visibility.Visible; }
        if (_progress is not null) { _progress.Value = request.Progress; _progress.Visibility = request.Progress >= 1 ? Visibility.Collapsed : Visibility.Visible; }
        if (_root is not null) AutomationProperties.SetName(_root, $"Achievement unlocked: {request.Title}");
    }
}
