using System.Collections.Immutable;
using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

/// <summary>
/// Coordinates queue intent and a host-owned playback session for Now Playing surfaces.
/// The controller never opens media, creates windows, or chooses a concrete playback engine.
/// </summary>
public sealed class NowPlayingController : IDisposable
{
    private readonly SemaphoreSlim _commandGate = new(1, 1);
    private readonly object _stateGate = new();
    private readonly NowPlayingQueue _queue;
    private IMediaPlaybackSession? _session;
    private CancellationTokenSource? _sessionCancellation;
    private long _lastSessionRevision = -1;
    private long _revision;
    private bool _disposed;
    private NowPlayingPresentationMode _presentationMode = NowPlayingPresentationMode.Full;
    private NowPlayingRepeatMode _repeatMode;
    private bool _shuffle;
    private bool _queueOpen;
    private bool _isBusy;
    private bool _isAutoAdvanceEnabled = true;
    private long _lastHandledEndedRevision = -1;
    private string? _pendingItemId;

    public NowPlayingController(NowPlayingQueue? queue = null)
    {
        _queue = queue ?? new NowPlayingQueue();
        _queue.Changed += OnQueueChanged;
        CurrentState = CreateState();
    }

    public NowPlayingQueue Queue => _queue;
    public IMediaPlaybackSession? PlaybackSession => _session;
    public NowPlayingState CurrentState { get; private set; }
    public bool IsAutoAdvanceEnabled
    {
        get => _isAutoAdvanceEnabled;
        set
        {
            ThrowIfDisposed();
            if (_isAutoAdvanceEnabled == value) return;
            _isAutoAdvanceEnabled = value;
            PublishState();
        }
    }

    public event EventHandler<NowPlayingStateChangedEventArgs>? StateChanged;
    public event EventHandler<NowPlayingCommandCompletedEventArgs>? CommandCompleted;
    public event EventHandler<NowPlayingItemActivationRequestedEventArgs>? ItemActivationRequested;

    public void SetPresentationMode(NowPlayingPresentationMode mode)
    {
        ThrowIfDisposed();
        if (_presentationMode == mode) return;
        _presentationMode = mode;
        PublishState();
    }

    public void SetQueueOpen(bool isOpen)
    {
        ThrowIfDisposed();
        if (_queueOpen == isOpen) return;
        _queueOpen = isOpen;
        PublishState();
    }

    public void SetRepeatMode(NowPlayingRepeatMode mode)
    {
        ThrowIfDisposed();
        if (_repeatMode == mode) return;
        _repeatMode = mode;
        PublishState();
        PublishCommand(NowPlayingCommandKind.SetRepeatMode, MediaPlaybackCommandResult.Success);
    }

    public void SetShuffleEnabled(bool enabled)
    {
        ThrowIfDisposed();
        if (_shuffle == enabled) return;
        _shuffle = enabled;
        PublishState();
        PublishCommand(NowPlayingCommandKind.ToggleShuffle, MediaPlaybackCommandResult.Success);
    }

    /// <summary>Attaches an existing host session and optionally selects its queue item.</summary>
    public void AttachSession(IMediaPlaybackSession? session, NowPlayingMediaItem? item = null)
    {
        ThrowIfDisposed();
        DetachSession();

        if (item is not null)
        {
            _queue.TrySelect(item.Id);
            if (_queue.SelectedItem is null) _queue.Enqueue(item);
            _queue.TrySelect(item.Id);
        }

        if (session is null)
        {
            PublishState();
            return;
        }

        _session = session;
        _sessionCancellation = new CancellationTokenSource();
        _lastSessionRevision = session.CurrentSnapshot.Revision;
        _lastHandledEndedRevision = -1;
        session.SnapshotChanged += OnSessionSnapshotChanged;
        PublishState(session.CurrentSnapshot);
    }

    public void DetachSession()
    {
        _sessionCancellation?.Cancel();
        _sessionCancellation?.Dispose();
        _sessionCancellation = null;

        if (_session is not null)
        {
            _session.SnapshotChanged -= OnSessionSnapshotChanged;
            _session = null;
        }

        _lastSessionRevision = -1;
        _lastHandledEndedRevision = -1;
        _pendingItemId = null;
        if (!_disposed) PublishState();
    }

    public Task<MediaPlaybackCommandResult> PlayAsync(CancellationToken cancellationToken = default) =>
        ExecutePlaybackAsync(NowPlayingCommandKind.Play,
            static (session, token) => session.PlayAsync(token), cancellationToken);

    public Task<MediaPlaybackCommandResult> PauseAsync(CancellationToken cancellationToken = default) =>
        ExecutePlaybackAsync(NowPlayingCommandKind.Pause,
            static (session, token) => session.PauseAsync(token), cancellationToken);

    public Task<MediaPlaybackCommandResult> StopAsync(CancellationToken cancellationToken = default) =>
        ExecutePlaybackAsync(NowPlayingCommandKind.Stop,
            static (session, token) => session.StopAsync(token), cancellationToken);

    public Task<MediaPlaybackCommandResult> SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
    {
        if (position < TimeSpan.Zero) return Task.FromResult(MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument));
        return ExecutePlaybackAsync(NowPlayingCommandKind.Seek,
            (session, token) => session.SeekAsync(position, token), cancellationToken);
    }

    public Task<MediaPlaybackCommandResult> ToggleAsync(CancellationToken cancellationToken = default)
    {
        var state = _session?.CurrentSnapshot.State;
        return state is MediaPlaybackState.Playing or MediaPlaybackState.Buffering
            ? PauseAsync(cancellationToken)
            : PlayAsync(cancellationToken);
    }

    /// <summary>Requests activation of the next queue item; the host binds its session.</summary>
    public Task<MediaPlaybackCommandResult> NextAsync(CancellationToken cancellationToken = default)
    {
        var index = GetNextIndex();
        return RequestQueueActivationAsync(NowPlayingCommandKind.Next, index, cancellationToken);
    }

    /// <summary>Requests activation of the previous queue item; the host binds its session.</summary>
    public Task<MediaPlaybackCommandResult> PreviousAsync(CancellationToken cancellationToken = default)
    {
        var index = GetPreviousIndex();
        return RequestQueueActivationAsync(NowPlayingCommandKind.Previous, index, cancellationToken);
    }

    public Task<MediaPlaybackCommandResult> SelectQueueItemAsync(int index, CancellationToken cancellationToken = default)
    {
        if (!_queue.TrySelect(index))
        {
            var rejected = MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument);
            PublishCommand(NowPlayingCommandKind.SelectQueueItem, rejected);
            return Task.FromResult(rejected);
        }

        return RequestQueueActivationAsync(NowPlayingCommandKind.SelectQueueItem, index, cancellationToken);
    }

    public NowPlayingMediaItem? GetNextItem() => GetItemAt(GetNextIndex());
    public NowPlayingMediaItem? GetPreviousItem() => GetItemAt(GetPreviousIndex());

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _queue.Changed -= OnQueueChanged;
        DetachSession();
        // Do not dispose the gate while an in-flight UI command may still be unwinding;
        // the controller is unreachable after Dispose and the tiny semaphore is reclaimed
        // with it. This keeps late command completions deterministic.
    }

    private int GetNextIndex()
    {
        var next = _queue.GetNextIndex(_repeatMode);
        if (!_shuffle || _queue.Items.Length <= 1 || next < 0) return next;

        // A deterministic neighbour selection keeps state reproducible. Hosts can implement
        // a richer shuffle policy by reordering the queue through TryMove.
        var selected = _queue.SelectedIndex;
        return (selected + 1) % _queue.Items.Length;
    }

    private int GetPreviousIndex()
    {
        var previous = _queue.GetPreviousIndex(_repeatMode);
        if (!_shuffle || _queue.Items.Length <= 1 || previous < 0) return previous;
        var selected = _queue.SelectedIndex;
        return selected <= 0 ? _queue.Items.Length - 1 : selected - 1;
    }

    private async Task<MediaPlaybackCommandResult> RequestQueueActivationAsync(
        NowPlayingCommandKind kind,
        int index,
        CancellationToken cancellationToken)
    {
        if (_disposed || cancellationToken.IsCancellationRequested)
        {
            var cancelled = MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed);
            PublishCommand(kind, cancelled);
            return cancelled;
        }
        if (index < 0 || index >= _queue.Items.Length)
        {
            var rejected = MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.Unsupported);
            PublishCommand(kind, rejected);
            return rejected;
        }

        var item = _queue.Items[index];
        _queue.TrySelect(index);
        _pendingItemId = item.Id;
        PublishState();
        var args = new NowPlayingItemActivationRequestedEventArgs(item, index);
        MediaPlaybackCommandResult result;
        try
        {
            ItemActivationRequested?.Invoke(this, args);
            result = args.Handled
                ? MediaPlaybackCommandResult.Success
                : MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.Unsupported);
        }
        catch
        {
            _pendingItemId = null;
            result = MediaPlaybackCommandResult.Failed(MediaPlaybackErrorCode.EngineFailure);
        }
        if (result.Status != MediaPlaybackCommandStatus.Success) _pendingItemId = null;
        PublishState();
        PublishCommand(kind, result);
        await Task.CompletedTask;
        return result;
    }

    private async Task<MediaPlaybackCommandResult> ExecutePlaybackAsync(
        NowPlayingCommandKind kind,
        Func<IMediaPlaybackSession, CancellationToken, Task<MediaPlaybackCommandResult>> command,
        CancellationToken cancellationToken)
    {
        var session = _session;
        if (session is null)
        {
            var rejected = MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.Unsupported);
            PublishCommand(kind, rejected);
            return rejected;
        }

        try
        {
            await _commandGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            _isBusy = true;
            PublishState();
            try
            {
                if (_disposed || !ReferenceEquals(session, _session))
                {
                    var cancelled = MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed);
                    PublishCommand(kind, cancelled);
                    return cancelled;
                }

                var result = await command(session, cancellationToken).ConfigureAwait(false);
                PublishCommand(kind, result);
                return result;
            }
            finally
            {
                _isBusy = false;
                PublishState();
                _commandGate.Release();
            }
        }
        catch (OperationCanceledException)
        {
            var cancelled = MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed);
            PublishCommand(kind, cancelled);
            return cancelled;
        }
        catch
        {
            var failed = MediaPlaybackCommandResult.Failed(MediaPlaybackErrorCode.EngineFailure);
            PublishCommand(kind, failed);
            return failed;
        }
    }

    private void OnQueueChanged(object? sender, EventArgs e) => PublishState();

    private void OnSessionSnapshotChanged(object? sender, MediaPlaybackSessionChangedEventArgs args)
    {
        if (_disposed || !ReferenceEquals(sender, _session)) return;
        lock (_stateGate)
        {
            if (args.Snapshot.Revision <= _lastSessionRevision) return;
            _lastSessionRevision = args.Snapshot.Revision;
        }
        PublishState(args.Snapshot);
        if (_isAutoAdvanceEnabled && args.Snapshot.State == MediaPlaybackState.Ended && args.Snapshot.Revision > _lastHandledEndedRevision)
        {
            _lastHandledEndedRevision = args.Snapshot.Revision;
            _ = NextAsync();
        }
    }

    private void PublishCommand(NowPlayingCommandKind kind, MediaPlaybackCommandResult result)
    {
        CommandCompleted?.Invoke(this, new NowPlayingCommandCompletedEventArgs(kind, result, CurrentState.Revision));
    }

    private void PublishState(MediaPlaybackSnapshot? snapshot = null)
    {
        if (_disposed) return;
        var state = CreateState(snapshot ?? _session?.CurrentSnapshot);
        CurrentState = state;
        StateChanged?.Invoke(this, new NowPlayingStateChangedEventArgs(state));
    }

    private NowPlayingState CreateState(MediaPlaybackSnapshot? snapshot = null)
    {
        var items = _queue.Items;
        var index = _queue.SelectedIndex;
        var item = index >= 0 && index < items.Length ? items[index] : null;
        return new NowPlayingState(
            Interlocked.Increment(ref _revision),
            item,
            index,
            items,
            snapshot,
            _presentationMode,
            _repeatMode,
            _shuffle,
            _queueOpen,
            _isBusy,
            _isAutoAdvanceEnabled,
            _pendingItemId);
    }

    private NowPlayingMediaItem? GetItemAt(int index) => index >= 0 && index < _queue.Items.Length ? _queue.Items[index] : null;

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(NowPlayingController));
    }
}
