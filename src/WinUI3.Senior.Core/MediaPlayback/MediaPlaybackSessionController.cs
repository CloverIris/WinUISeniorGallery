namespace WinUI3.Senior.Core;

/// <summary>
/// Host-owned façade that serializes playback commands and filters stale snapshots when
/// a media source is replaced. It never creates a player, owns a file, or chooses a window.
/// </summary>
public sealed class MediaPlaybackSessionController : IMediaPlaybackSession, IDisposable
{
    private readonly SemaphoreSlim _commandGate = new(1, 1);
    private readonly object _sync = new();
    private IMediaPlaybackSession? _session;
    private MediaPlaybackSnapshot? _snapshot;
    private bool _isDisposed;

    public MediaPlaybackSessionController(IMediaPlaybackSession? session = null)
    {
        ReplaceSession(session);
    }

    public MediaPlaybackSnapshot CurrentSnapshot
    {
        get
        {
            lock (_sync)
            {
                return _snapshot ?? CreateUnavailableSnapshot();
            }
        }
    }

    public event EventHandler<MediaPlaybackSessionChangedEventArgs>? SnapshotChanged;

    /// <summary>Replaces the host-owned session. Commands already in flight finish against the old session.</summary>
    public void ReplaceSession(IMediaPlaybackSession? session)
    {
        IMediaPlaybackSession? previous;
        lock (_sync)
        {
            ThrowIfDisposed();
            previous = _session;
            if (ReferenceEquals(previous, session))
            {
                return;
            }

            if (previous is not null)
            {
                previous.SnapshotChanged -= OnSnapshotChanged;
            }

            _session = session;
            _snapshot = session?.CurrentSnapshot;
        }

        if (session is not null)
        {
            session.SnapshotChanged += OnSnapshotChanged;
            PublishSnapshot(session.CurrentSnapshot, force: true);
        }
        else
        {
            PublishSnapshot(CreateUnavailableSnapshot(), force: true);
        }
    }

    public Task<MediaPlaybackCommandResult> PlayAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(static (session, token) => session.PlayAsync(token), cancellationToken);

    public Task<MediaPlaybackCommandResult> PauseAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(static (session, token) => session.PauseAsync(token), cancellationToken);

    public Task<MediaPlaybackCommandResult> StopAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(static (session, token) => session.StopAsync(token), cancellationToken);

    public Task<MediaPlaybackCommandResult> SeekAsync(TimeSpan position, CancellationToken cancellationToken = default) =>
        ExecuteAsync((session, token) => session.SeekAsync(position, token), cancellationToken);

    public Task<MediaPlaybackCommandResult> SetVolumeAsync(double volume, CancellationToken cancellationToken = default) =>
        ExecuteAsync((session, token) => session.SetVolumeAsync(volume, token), cancellationToken);

    public Task<MediaPlaybackCommandResult> SetPlaybackRateAsync(double playbackRate, CancellationToken cancellationToken = default) =>
        ExecuteAsync((session, token) => session.SetPlaybackRateAsync(playbackRate, token), cancellationToken);

    private async Task<MediaPlaybackCommandResult> ExecuteAsync(
        Func<IMediaPlaybackSession, CancellationToken, Task<MediaPlaybackCommandResult>> command,
        CancellationToken cancellationToken)
    {
        IMediaPlaybackSession? session;
        lock (_sync)
        {
            if (_isDisposed)
            {
                return MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed);
            }

            session = _session;
        }

        if (session is null)
        {
            return MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.Unsupported);
        }

        try
        {
            await _commandGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                lock (_sync)
                {
                    if (_isDisposed || !ReferenceEquals(session, _session))
                    {
                        return MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed);
                    }
                }

                return await command(session, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _commandGate.Release();
            }
        }
        catch (OperationCanceledException)
        {
            return MediaPlaybackCommandResult.Cancelled(MediaPlaybackErrorCode.Disposed);
        }
        catch
        {
            return MediaPlaybackCommandResult.Failed(MediaPlaybackErrorCode.EngineFailure);
        }
    }

    private void OnSnapshotChanged(object? sender, MediaPlaybackSessionChangedEventArgs args)
    {
        lock (_sync)
        {
            if (_isDisposed || !ReferenceEquals(sender, _session))
            {
                return;
            }
        }

        PublishSnapshot(args.Snapshot);
    }

    private void PublishSnapshot(MediaPlaybackSnapshot snapshot, bool force = false)
    {
        EventHandler<MediaPlaybackSessionChangedEventArgs>? handler;
        lock (_sync)
        {
            if (_isDisposed)
            {
                return;
            }

            if (!force && _snapshot is not null &&
                (!Equals(_snapshot.SessionId, snapshot.SessionId) || snapshot.Revision <= _snapshot.Revision))
            {
                return;
            }

            _snapshot = snapshot;
            handler = SnapshotChanged;
        }

        handler?.Invoke(this, new MediaPlaybackSessionChangedEventArgs(snapshot));
    }

    private static MediaPlaybackSnapshot CreateUnavailableSnapshot() => new(
        new PlaybackSessionId("controller-unavailable"),
        0,
        MediaPlaybackState.Unavailable,
        MediaPlaybackMode.Unknown,
        MediaPlaybackCapabilities.None,
        TimeSpan.Zero,
        new MediaPlaybackTimeRange(TimeSpan.Zero, TimeSpan.Zero),
        Array.Empty<MediaPlaybackTimeRange>(),
        1,
        1);

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(MediaPlaybackSessionController));
        }
    }

    public void Dispose()
    {
        IMediaPlaybackSession? session;
        lock (_sync)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            session = _session;
            _session = null;
            _snapshot = null;
        }

        if (session is not null)
        {
            session.SnapshotChanged -= OnSnapshotChanged;
        }

        // Do not dispose the semaphore while a command may still be inside its finally block.
        // The controller is unreachable after disposal and the semaphore will be reclaimed with
        // it; keeping it alive makes late command completion deterministic and avoids a
        // SemaphoreFull/Disposed exception on Release.
    }
}
