using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Controls;

public enum FocusSessionState
{
    Idle,
    Running,
    Paused,
    Break,
    Completed,
    Cancelled
}

public enum FocusSessionPhase
{
    Focus,
    ShortBreak,
    LongBreak
}

public sealed record FocusSessionTask(string Id, string Title, bool IsCompleted = false, object? Tag = null)
{
    public FocusSessionTask WithCompletion(bool completed) => this with { IsCompleted = completed };
}

public sealed record FocusSessionSnapshot(
    long Revision,
    FocusSessionState State,
    FocusSessionPhase Phase,
    int Cycle,
    TimeSpan Remaining,
    TimeSpan Total,
    IReadOnlyList<FocusSessionTask> Tasks,
    string? ActiveTaskId)
{
    public double Progress => Total <= TimeSpan.Zero
        ? 0
        : Math.Clamp(1d - Remaining.TotalMilliseconds / Total.TotalMilliseconds, 0, 1);
}

public sealed class FocusSessionStateChangedEventArgs(FocusSessionSnapshot snapshot) : EventArgs
{
    public FocusSessionSnapshot Snapshot { get; } = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
}

public sealed class FocusSessionPhaseChangedEventArgs(FocusSessionPhase previous, FocusSessionPhase current, int cycle) : EventArgs
{
    public FocusSessionPhase Previous { get; } = previous;
    public FocusSessionPhase Current { get; } = current;
    public int Cycle { get; } = cycle;
}

/// <summary>
/// Host-neutral Focus Sessions timer. It owns phase transitions and task ordering only;
/// persistence, notifications, sound and Do-Not-Disturb integration remain host-owned.
/// </summary>
public sealed class FocusSession : Control, IDisposable, INotifyPropertyChanged
{
    private readonly DispatcherQueueTimer _timer;
    private TimeSpan _remaining;
    private TimeSpan _total;
    private FocusSessionPhase _phase = FocusSessionPhase.Focus;
    private FocusSessionState _state = FocusSessionState.Idle;
    private int _cycle;
    private long _revision;
    private bool _disposed;
    private bool _isTransitioning;
    private FocusSessionSnapshot _snapshot = null!;

    public static readonly DependencyProperty WorkDurationProperty = DependencyProperty.Register(
        nameof(WorkDuration), typeof(TimeSpan), typeof(FocusSession), new PropertyMetadata(TimeSpan.FromMinutes(25), OnDurationChanged));
    public static readonly DependencyProperty ShortBreakDurationProperty = DependencyProperty.Register(
        nameof(ShortBreakDuration), typeof(TimeSpan), typeof(FocusSession), new PropertyMetadata(TimeSpan.FromMinutes(5), OnDurationChanged));
    public static readonly DependencyProperty LongBreakDurationProperty = DependencyProperty.Register(
        nameof(LongBreakDuration), typeof(TimeSpan), typeof(FocusSession), new PropertyMetadata(TimeSpan.FromMinutes(15), OnDurationChanged));
    public static readonly DependencyProperty CyclesBeforeLongBreakProperty = DependencyProperty.Register(
        nameof(CyclesBeforeLongBreak), typeof(int), typeof(FocusSession), new PropertyMetadata(4));
    public static readonly DependencyProperty IsHostVisibleProperty = DependencyProperty.Register(
        nameof(IsHostVisible), typeof(bool), typeof(FocusSession), new PropertyMetadata(true, OnHostStateChanged));
    public static readonly DependencyProperty IsHostWindowActiveProperty = DependencyProperty.Register(
        nameof(IsHostWindowActive), typeof(bool), typeof(FocusSession), new PropertyMetadata(true, OnHostStateChanged));
    public static readonly DependencyProperty IsAutoStartBreakEnabledProperty = DependencyProperty.Register(
        nameof(IsAutoStartBreakEnabled), typeof(bool), typeof(FocusSession), new PropertyMetadata(false));
    public static readonly DependencyProperty ActiveTaskIdProperty = DependencyProperty.Register(
        nameof(ActiveTaskId), typeof(string), typeof(FocusSession), new PropertyMetadata(null, OnActiveTaskChanged));

    public FocusSession()
    {
        DefaultStyleKey = typeof(FocusSession);
        _timer = DispatcherQueue.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
        Tasks.CollectionChanged += OnTasksChanged;
        Loaded += (_, _) => ApplyHostPause();
        Unloaded += (_, _) => PauseForHost();
        Reset();
    }

    public TimeSpan WorkDuration { get => (TimeSpan)GetValue(WorkDurationProperty); set => SetValue(WorkDurationProperty, value); }
    public TimeSpan ShortBreakDuration { get => (TimeSpan)GetValue(ShortBreakDurationProperty); set => SetValue(ShortBreakDurationProperty, value); }
    public TimeSpan LongBreakDuration { get => (TimeSpan)GetValue(LongBreakDurationProperty); set => SetValue(LongBreakDurationProperty, value); }
    public int CyclesBeforeLongBreak { get => (int)GetValue(CyclesBeforeLongBreakProperty); set => SetValue(CyclesBeforeLongBreakProperty, Math.Max(1, value)); }
    public bool IsHostVisible { get => (bool)GetValue(IsHostVisibleProperty); set => SetValue(IsHostVisibleProperty, value); }
    public bool IsHostWindowActive { get => (bool)GetValue(IsHostWindowActiveProperty); set => SetValue(IsHostWindowActiveProperty, value); }
    public bool IsAutoStartBreakEnabled { get => (bool)GetValue(IsAutoStartBreakEnabledProperty); set => SetValue(IsAutoStartBreakEnabledProperty, value); }
    public string? ActiveTaskId { get => (string?)GetValue(ActiveTaskIdProperty); set => SetValue(ActiveTaskIdProperty, value); }
    public ObservableCollection<FocusSessionTask> Tasks { get; } = new();
    public FocusSessionSnapshot Snapshot => _snapshot;
    public FocusSessionState State => _state;
    public FocusSessionPhase Phase => _phase;
    public TimeSpan Remaining => _remaining;
    public int Cycle => _cycle;

    public event EventHandler<FocusSessionStateChangedEventArgs>? StateChanged;
    public event EventHandler<FocusSessionPhaseChangedEventArgs>? PhaseChanged;
    public event EventHandler? SessionCompleted;
    public event EventHandler? SessionCancelled;
    public event EventHandler? Tick;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    public bool Start()
    {
        if (_disposed || _state is FocusSessionState.Running or FocusSessionState.Break) return false;
        if (_state is FocusSessionState.Completed or FocusSessionState.Cancelled) Reset();
        _state = FocusSessionState.Running;
        ApplyHostPause();
        Publish();
        return true;
    }

    public bool Pause()
    {
        if (_disposed || _state != FocusSessionState.Running) return false;
        _state = FocusSessionState.Paused;
        _timer.Stop();
        Publish();
        return true;
    }

    public bool Resume()
    {
        if (_disposed || _state != FocusSessionState.Paused) return false;
        _state = _phase == FocusSessionPhase.Focus ? FocusSessionState.Running : FocusSessionState.Break;
        ApplyHostPause();
        Publish();
        return true;
    }

    public bool SkipPhase()
    {
        if (_disposed || _state is FocusSessionState.Idle or FocusSessionState.Completed or FocusSessionState.Cancelled) return false;
        AdvancePhase();
        return true;
    }

    public void Cancel()
    {
        if (_disposed) return;
        _timer.Stop();
        _state = FocusSessionState.Cancelled;
        Publish();
        SessionCancelled?.Invoke(this, EventArgs.Empty);
    }

    public void Reset()
    {
        if (_disposed) return;
        _timer.Stop();
        _phase = FocusSessionPhase.Focus;
        _cycle = 0;
        _state = FocusSessionState.Idle;
        _total = NormalizeDuration(WorkDuration, TimeSpan.FromMinutes(25));
        _remaining = _total;
        Publish();
    }

    public void ReplaceTasks(IEnumerable<FocusSessionTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        var values = tasks.ToArray();
        if (values.Any(task => string.IsNullOrWhiteSpace(task.Id)))
            throw new ArgumentException("Focus task ids cannot be empty.", nameof(tasks));
        if (values.Select(task => task.Id).Distinct(StringComparer.Ordinal).Count() != values.Length)
            throw new ArgumentException("Focus task ids must be unique.", nameof(tasks));
        Tasks.Clear();
        foreach (var task in values) Tasks.Add(task);
        if (ActiveTaskId is null || !values.Any(task => string.Equals(task.Id, ActiveTaskId, StringComparison.Ordinal)))
            ActiveTaskId = values.FirstOrDefault()?.Id;
        Publish();
    }

    public bool CompleteActiveTask()
    {
        if (string.IsNullOrWhiteSpace(ActiveTaskId)) return false;
        var index = Tasks.ToList().FindIndex(task => string.Equals(task.Id, ActiveTaskId, StringComparison.Ordinal));
        if (index < 0 || Tasks[index].IsCompleted) return false;
        Tasks[index] = Tasks[index].WithCompletion(true);
        Publish();
        return true;
    }

    private void OnTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (_disposed || _state is not (FocusSessionState.Running or FocusSessionState.Break) || _isTransitioning) return;
        if (!IsHostVisible || !IsHostWindowActive) return;
        _remaining -= TimeSpan.FromSeconds(1);
        Tick?.Invoke(this, EventArgs.Empty);
        if (_remaining <= TimeSpan.Zero) AdvancePhase(); else Publish();
    }

    private void AdvancePhase()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        try
        {
            _timer.Stop();
            var previous = _phase;
            if (_phase == FocusSessionPhase.Focus)
            {
                _cycle++;
                _phase = _cycle % Math.Max(1, CyclesBeforeLongBreak) == 0 ? FocusSessionPhase.LongBreak : FocusSessionPhase.ShortBreak;
            }
            else
            {
                _phase = FocusSessionPhase.Focus;
            }
            _total = DurationFor(_phase);
            _remaining = _total;
            _state = _phase == FocusSessionPhase.Focus || IsAutoStartBreakEnabled
                ? (_phase == FocusSessionPhase.Focus ? FocusSessionState.Running : FocusSessionState.Break)
                : FocusSessionState.Paused;
            PhaseChanged?.Invoke(this, new FocusSessionPhaseChangedEventArgs(previous, _phase, _cycle));
            if (_cycle > 0 && _phase == FocusSessionPhase.Focus && Tasks.Count > 0 && Tasks.All(task => task.IsCompleted))
            {
                _state = FocusSessionState.Completed;
                SessionCompleted?.Invoke(this, EventArgs.Empty);
            }
            ApplyHostPause();
            Publish();
        }
        finally { _isTransitioning = false; }
    }

    private void ApplyHostPause()
    {
        if (_state is (FocusSessionState.Running or FocusSessionState.Break) && IsHostVisible && IsHostWindowActive)
            _timer.Start();
        else _timer.Stop();
    }

    private void PauseForHost()
    {
        _timer.Stop();
    }

    private static TimeSpan DurationForPhase(FocusSessionPhase phase, TimeSpan work, TimeSpan shortBreak, TimeSpan longBreak) => phase switch
    {
        FocusSessionPhase.ShortBreak => shortBreak,
        FocusSessionPhase.LongBreak => longBreak,
        _ => work
    };

    private TimeSpan DurationFor(FocusSessionPhase phase) => DurationForPhase(phase, NormalizeDuration(WorkDuration, TimeSpan.FromMinutes(25)), NormalizeDuration(ShortBreakDuration, TimeSpan.FromMinutes(5)), NormalizeDuration(LongBreakDuration, TimeSpan.FromMinutes(15)));
    private static TimeSpan NormalizeDuration(TimeSpan value, TimeSpan fallback) => value >= TimeSpan.FromSeconds(1) && value <= TimeSpan.FromHours(24) ? value : fallback;

    private void Publish()
    {
        if (_disposed) return;
        var snapshot = CreateSnapshot();
        _snapshot = snapshot;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snapshot)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Phase)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Remaining)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cycle)));
        StateChanged?.Invoke(this, new FocusSessionStateChangedEventArgs(snapshot));
        VisualStateManager.GoToState(this, _state.ToString(), true);
    }

    private FocusSessionSnapshot CreateSnapshot() => new(
        Interlocked.Increment(ref _revision),
        _state,
        _phase,
        _cycle,
        _remaining,
        _total,
        Tasks.ToArray(),
        ActiveTaskId);

    private static void OnDurationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var session = (FocusSession)sender;
        if (session._state == FocusSessionState.Idle) session.Reset();
    }

    private static void OnHostStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) => ((FocusSession)sender).ApplyHostPause();

    private static void OnActiveTaskChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var session = (FocusSession)sender;
        if (!session._disposed)
        {
            session.Publish();
        }
    }

    private void OnTasksChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args) => Publish();

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        Tasks.CollectionChanged -= OnTasksChanged;
    }
}
