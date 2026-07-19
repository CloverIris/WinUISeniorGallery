using System.Collections.Concurrent;
using System.Windows.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Controls;

public enum SnackbarPriority { Low, Normal, High, Critical }
public enum SnackbarVisualKind { Informational, Success, Warning, Error }
public enum SnackbarCompletionReason { TimedOut, ActionInvoked, Dismissed, Cancelled, HostDestroyed, Replaced }
public enum SnackbarPlacement { BottomLeft, BottomCenter, BottomRight, TopLeft, TopCenter, TopRight }

public sealed record SnackbarAction
{
    public SnackbarAction(string label, ICommand command, object? commandParameter = null)
    {
        if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException("Action label is required.", nameof(label));
        Label = label;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CommandParameter = commandParameter;
    }
    public string Label { get; }
    public ICommand Command { get; }
    public object? CommandParameter { get; }
}

public sealed record SnackbarRequest
{
    public SnackbarRequest(Guid id, string message, SnackbarVisualKind kind = SnackbarVisualKind.Informational, SnackbarPriority priority = SnackbarPriority.Normal,
        TimeSpan? duration = null, SnackbarAction? action = null, string? deduplicationKey = null, bool isDismissible = true, string? automationAnnouncement = null) : this()
    {
        if (id == Guid.Empty) throw new ArgumentException("Request id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message is required.", nameof(message));
        if (duration is { } value && (value < TimeSpan.FromSeconds(1) || value > TimeSpan.FromSeconds(60))) throw new ArgumentOutOfRangeException(nameof(duration));
        Id = id; Message = message; Kind = kind; Priority = priority; Duration = duration; Action = action; DeduplicationKey = deduplicationKey; IsDismissible = isDismissible; AutomationAnnouncement = automationAnnouncement;
    }
    public Guid Id { get; }
    public string Message { get; }
    public SnackbarVisualKind Kind { get; }
    public SnackbarPriority Priority { get; }
    public TimeSpan? Duration { get; }
    public SnackbarAction? Action { get; }
    public string? DeduplicationKey { get; }
    public bool IsDismissible { get; }
    public string? AutomationAnnouncement { get; }
}

public sealed record SnackbarResult(Guid RequestId, SnackbarCompletionReason Reason);

public sealed class SnackbarShownEventArgs : EventArgs
{
    public SnackbarShownEventArgs(SnackbarRequest request) => Request = request;
    public SnackbarRequest Request { get; }
}

public sealed class SnackbarClosedEventArgs : EventArgs
{
    public SnackbarClosedEventArgs(SnackbarRequest request, SnackbarCompletionReason reason) { Request = request; Reason = reason; }
    public SnackbarRequest Request { get; }
    public SnackbarCompletionReason Reason { get; }
}

public sealed class SnackbarHostRegistration : IDisposable
{
    private SnackbarService? _service;
    internal SnackbarHostRegistration(SnackbarService service, Microsoft.UI.WindowId windowId, SnackbarHost host, string? hostName) { _service = service; WindowId = windowId; Host = host; HostName = hostName; }
    public Microsoft.UI.WindowId WindowId { get; }
    public string? HostName { get; }
    public SnackbarHost Host { get; }
    public bool IsValid => _service is not null && Host.IsRegistered;
    public void Dispose() => _service?.Unregister(this);
    internal void Invalidate() => _service = null;
}

public interface ISnackbarService
{
    SnackbarHostRegistration Register(Microsoft.UI.WindowId windowId, SnackbarHost host, string? hostName = null);
    Task<SnackbarResult> ShowAsync(SnackbarHostRegistration target, SnackbarRequest request, CancellationToken cancellationToken = default);
    bool Cancel(SnackbarHostRegistration target, Guid requestId);
}

/// <summary>Window-scoped snackbar queue. The service never chooses another window when a target is invalid.</summary>
public sealed class SnackbarService : ISnackbarService
{
    private readonly ConcurrentDictionary<(Microsoft.UI.WindowId, string?), SnackbarHostRegistration> _registrations = new();
    public SnackbarHostRegistration Register(Microsoft.UI.WindowId windowId, SnackbarHost host, string? hostName = null)
    {
        ArgumentNullException.ThrowIfNull(host);
        var registration = new SnackbarHostRegistration(this, windowId, host, hostName);
        if (!_registrations.TryAdd((windowId, hostName), registration)) throw new InvalidOperationException("A snackbar host with this window/name is already registered.");
        host.AttachRegistration(registration);
        return registration;
    }
    public Task<SnackbarResult> ShowAsync(SnackbarHostRegistration target, SnackbarRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(target); ArgumentNullException.ThrowIfNull(request);
        if (!target.IsValid) return Task.FromResult(new SnackbarResult(request.Id, SnackbarCompletionReason.HostDestroyed));
        return target.Host.EnqueueAsync(request, cancellationToken);
    }
    public bool Cancel(SnackbarHostRegistration target, Guid requestId) => target.IsValid && target.Host.Cancel(requestId);
    internal void Unregister(SnackbarHostRegistration registration)
    {
        _registrations.TryRemove((registration.WindowId, registration.HostName), out _);
        registration.Host.DestroyHost();
    }
}

[TemplatePart(Name = "PART_ActionButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_DismissButton", Type = typeof(Button))]
public sealed class SnackbarHost : Control
{
    private readonly object _gate = new();
    private readonly List<QueueEntry> _queue = new();
    private DispatcherQueueTimer? _timer;
    private QueueEntry? _current;
    private SnackbarHostRegistration? _registration;
    private bool _destroyed;
    private TimeSpan _remaining;
    private DateTimeOffset _timerStartedAt;
    private readonly Dictionary<Guid, QueueEntry> _entries = new();
    private Button? _actionButton;
    private Button? _dismissButton;

    public static readonly DependencyProperty HostNameProperty = DependencyProperty.Register(nameof(HostName), typeof(string), typeof(SnackbarHost), new PropertyMetadata(null));
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(SnackbarPlacement), typeof(SnackbarHost), new PropertyMetadata(SnackbarPlacement.BottomCenter));
    public static readonly DependencyProperty IsPausedProperty = DependencyProperty.Register(nameof(IsPaused), typeof(bool), typeof(SnackbarHost), new PropertyMetadata(false, OnPausedChanged));
    public static readonly DependencyProperty CurrentRequestProperty = DependencyProperty.Register(nameof(CurrentRequest), typeof(SnackbarRequest), typeof(SnackbarHost), new PropertyMetadata(null));

    public SnackbarHost() { DefaultStyleKey = typeof(SnackbarHost); Loaded += (_, _) => EnsureTimer(); Unloaded += (_, _) => DestroyHost(); KeyDown += (_, e) => { if (e.Key == Windows.System.VirtualKey.Escape) DismissCurrent(); }; }
    public string? HostName { get => (string?)GetValue(HostNameProperty); set => SetValue(HostNameProperty, value); }
    public SnackbarPlacement Placement { get => (SnackbarPlacement)GetValue(PlacementProperty); set => SetValue(PlacementProperty, value); }
    public bool IsPaused { get => (bool)GetValue(IsPausedProperty); set => SetValue(IsPausedProperty, value); }
    public SnackbarRequest? CurrentRequest { get => (SnackbarRequest?)GetValue(CurrentRequestProperty); private set => SetValue(CurrentRequestProperty, value); }
    internal bool IsRegistered => _registration is not null && !_destroyed;
    public bool IsShowing => _current is not null;
    public IReadOnlyList<SnackbarRequest> PendingRequests
    {
        get { lock (_gate) return _queue.Where(entry => !entry.IsCompleted).Select(entry => entry.Request).ToArray(); }
    }
    public event EventHandler<SnackbarShownEventArgs>? Shown;
    public event EventHandler<SnackbarClosedEventArgs>? Closed;
    public event EventHandler? QueueChanged;

    protected override void OnApplyTemplate()
    {
        if (_actionButton is not null) _actionButton.Click -= OnActionClick;
        if (_dismissButton is not null) _dismissButton.Click -= OnDismissClick;
        base.OnApplyTemplate();
        _actionButton = GetTemplateChild("PART_ActionButton") as Button;
        _dismissButton = GetTemplateChild("PART_DismissButton") as Button;
        if (_actionButton is not null) _actionButton.Click += OnActionClick;
        if (_dismissButton is not null) _dismissButton.Click += OnDismissClick;
        UpdateTemplateButtons();
    }

    internal void AttachRegistration(SnackbarHostRegistration registration) { _registration = registration; _destroyed = false; EnsureTimer(); }
    internal Task<SnackbarResult> EnqueueAsync(SnackbarRequest request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromResult(new SnackbarResult(request.Id, SnackbarCompletionReason.Cancelled));
        if (_destroyed) return Task.FromResult(new SnackbarResult(request.Id, SnackbarCompletionReason.HostDestroyed));
        var entry = new QueueEntry(request, cancellationToken);
        lock (_gate)
        {
            // Reusing an id is idempotent: callers observe the original request's
            // completion instead of waiting on an entry that was never enqueued.
            if (_entries.TryGetValue(request.Id, out var existing)) return existing.Completion.Task;
            if (request.DeduplicationKey is { Length: > 0 } key)
            {
                var duplicate = _entries.Values.FirstOrDefault(e => e.Request.DeduplicationKey == key);
                if (duplicate is not null) CompleteEntry(duplicate, SnackbarCompletionReason.Replaced);
            }
            _entries[request.Id] = entry; _queue.Add(entry); _queue.Sort((a, b) => b.Request.Priority != a.Request.Priority ? b.Request.Priority.CompareTo(a.Request.Priority) : a.Sequence.CompareTo(b.Sequence));
        }
        QueueChanged?.Invoke(this, EventArgs.Empty);
        cancellationToken.Register(() =>
        {
            var queue = DispatcherQueue;
            if (queue is not null) queue.TryEnqueue(() => Cancel(request.Id));
            else Cancel(request.Id);
        });
        EnsureTimer(); TryShowNext();
        return entry.Completion.Task;
    }
    internal bool Cancel(Guid id)
    {
        lock (_gate)
        {
            if (!_entries.TryGetValue(id, out var entry)) return false;
            CompleteEntry(entry, SnackbarCompletionReason.Cancelled);
            if (ReferenceEquals(_current, entry)) { _current = null; CurrentRequest = null; }
        }
        QueueChanged?.Invoke(this, EventArgs.Empty);
        TryShowNext(); return true;
    }
    public void DismissCurrent()
    {
        if (_current?.Request.IsDismissible != true) return;
        CompleteEntry(_current, SnackbarCompletionReason.Dismissed);
        QueueChanged?.Invoke(this, EventArgs.Empty);
        TryShowNext();
    }

    /// <summary>Removes queued, not-yet-visible messages while keeping the current message.</summary>
    public int ClearPending()
    {
        QueueEntry[] pending;
        lock (_gate) pending = _queue.Where(entry => !entry.IsCompleted).ToArray();
        foreach (var entry in pending) CompleteEntry(entry, SnackbarCompletionReason.Cancelled);
        if (pending.Length > 0) QueueChanged?.Invoke(this, EventArgs.Empty);
        return pending.Length;
    }

    public bool TryGetPending(Guid requestId, out SnackbarRequest? request)
    {
        lock (_gate)
        {
            if (_entries.TryGetValue(requestId, out var entry) && !entry.IsCompleted)
            {
                request = entry.Request;
                return true;
            }
        }
        request = null;
        return false;
    }
    public bool InvokeAction()
    {
        var entry = _current; if (entry is null || entry.Request.Action is null) return false;
        if (entry.ActionInvoked) return false; entry.ActionInvoked = true;
        CompleteEntry(entry, SnackbarCompletionReason.ActionInvoked);
        _ = InvokeCommandAsync(entry.Request.Action); TryShowNext(); return true;
    }
    internal void DestroyHost()
    {
        if (_destroyed) return; _destroyed = true; _timer?.Stop();
        lock (_gate) foreach (var entry in _entries.Values.ToArray()) CompleteEntry(entry, SnackbarCompletionReason.HostDestroyed);
        _entries.Clear(); _queue.Clear(); _current = null; CurrentRequest = null; _registration = null;
    }
    private async Task InvokeCommandAsync(SnackbarAction action)
    {
        try { if (action.Command.CanExecute(action.CommandParameter)) action.Command.Execute(action.CommandParameter); await Task.CompletedTask; } catch { /* command errors never change the committed result */ }
    }
    private void EnsureTimer() { _timer ??= DispatcherQueue.GetForCurrentThread().CreateTimer(); if (_timer is not null) { _timer.IsRepeating = false; _timer.Tick -= OnTimerTick; _timer.Tick += OnTimerTick; } }
    private void TryShowNext()
    {
        if (_destroyed || IsPaused || _current is not null) return;
        QueueEntry? next;
        lock (_gate) { next = _queue.FirstOrDefault(e => !e.IsCompleted); if (next is not null) _queue.Remove(next); }
        if (next is null) return;
        _current = next; CurrentRequest = next.Request; UpdateTemplateButtons(); next.StartedAt = DateTimeOffset.UtcNow; Shown?.Invoke(this, new SnackbarShownEventArgs(next.Request));
        QueueChanged?.Invoke(this, EventArgs.Empty);
        _remaining = next.Request.Duration ?? DefaultDuration(next.Request.Priority); _timerStartedAt = DateTimeOffset.UtcNow; _timer!.Interval = _remaining; _timer.Start();
    }
    private void OnTimerTick(DispatcherQueueTimer sender, object args) { if (!IsPaused && _current is not null) { CompleteEntry(_current, SnackbarCompletionReason.TimedOut); TryShowNext(); } }
    private void CompleteEntry(QueueEntry entry, SnackbarCompletionReason reason)
    {
        if (entry.IsCompleted) return; entry.IsCompleted = true; _entries.Remove(entry.Request.Id); _queue.Remove(entry);
        if (ReferenceEquals(_current, entry)) { _timer?.Stop(); _current = null; CurrentRequest = null; UpdateTemplateButtons(); }
        entry.Completion.TrySetResult(new SnackbarResult(entry.Request.Id, reason)); Closed?.Invoke(this, new SnackbarClosedEventArgs(entry.Request, reason)); QueueChanged?.Invoke(this, EventArgs.Empty);
    }
    private static TimeSpan DefaultDuration(SnackbarPriority priority) => priority switch { SnackbarPriority.Low or SnackbarPriority.Normal => TimeSpan.FromSeconds(5), SnackbarPriority.High => TimeSpan.FromSeconds(8), _ => TimeSpan.FromSeconds(10) };
    private void OnActionClick(object sender, RoutedEventArgs e) { if (InvokeAction()) e.Handled = true; }
    private void OnDismissClick(object sender, RoutedEventArgs e) { DismissCurrent(); e.Handled = true; }
    private void UpdateTemplateButtons()
    {
        if (_actionButton is not null)
        {
            _actionButton.Content = CurrentRequest?.Action?.Label ?? "Action";
            _actionButton.Visibility = CurrentRequest?.Action is null ? Visibility.Collapsed : Visibility.Visible;
            _actionButton.IsEnabled = CurrentRequest?.Action is not null && _current is { ActionInvoked: false };
        }
        if (_dismissButton is not null)
        {
            _dismissButton.Visibility = CurrentRequest?.IsDismissible == false ? Visibility.Collapsed : Visibility.Visible;
        }
    }
    private static void OnPausedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var host = (SnackbarHost)d;
        if (host._current is null) { if (!(bool)e.NewValue) host.TryShowNext(); return; }
        if ((bool)e.NewValue)
        {
            var elapsed = DateTimeOffset.UtcNow - host._timerStartedAt;
            host._remaining = host._remaining > elapsed ? host._remaining - elapsed : TimeSpan.Zero;
            host._timer?.Stop();
        }
        else
        {
            if (host._remaining <= TimeSpan.Zero) { host.CompleteEntry(host._current, SnackbarCompletionReason.TimedOut); host.TryShowNext(); }
            else { host._timerStartedAt = DateTimeOffset.UtcNow; host._timer!.Interval = host._remaining; host._timer.Start(); }
        }
    }
    private sealed class QueueEntry
    {
        private static long _nextSequence;
        public QueueEntry(SnackbarRequest request, CancellationToken token) { Request = request; Completion = new TaskCompletionSource<SnackbarResult>(TaskCreationOptions.RunContinuationsAsynchronously); Token = token; Sequence = Interlocked.Increment(ref _nextSequence); }
        public SnackbarRequest Request { get; }
        public CancellationToken Token { get; }
        public TaskCompletionSource<SnackbarResult> Completion { get; }
        public long Sequence { get; }
        public DateTimeOffset StartedAt { get; set; }
        public bool ActionInvoked { get; set; }
        public bool IsCompleted { get; set; }
    }
}
