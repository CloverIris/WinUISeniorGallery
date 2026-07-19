using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Media.Playback;
using WinUI3.Senior.Core;
using NativePlaybackState = Windows.Media.Playback.MediaPlaybackState;
using ContractPlaybackState = WinUI3.Senior.Core.MediaPlaybackState;

namespace WinUI3.Senior.Media;

/// <summary>
/// Adapts a host-owned <see cref="MediaPlayerElement"/> to the engine-neutral
/// <see cref="IMediaPlaybackSession"/> contract. It never creates a player,
/// changes the element's source, or takes ownership of the host's media source.
/// </summary>
public sealed class MediaPlayerElementPlaybackSessionAdapter : IMediaPlaybackSession, IDisposable
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly MediaPlayer _player;
    private readonly MediaPlaybackSession _playbackSession;
    private readonly PlaybackSessionId _sessionId = new(Guid.NewGuid().ToString("N"));
    private readonly TypedEventHandler<MediaPlaybackSession, object> _playbackStateChangedHandler;
    private readonly TypedEventHandler<MediaPlaybackSession, object> _positionChangedHandler;
    private readonly TypedEventHandler<MediaPlayer, object> _mediaOpenedHandler;
    private readonly TypedEventHandler<MediaPlayer, object> _mediaEndedHandler;
    private readonly TypedEventHandler<MediaPlayer, MediaPlayerFailedEventArgs> _mediaFailedHandler;
    private MediaPlaybackSnapshot _currentSnapshot;
    private long _revision;
    private int _generation;
    private bool _isDisposed;
    private bool _isEnded;
    private MediaPlaybackErrorCode _lastErrorCode;

    /// <summary>Creates an adapter for an element owned by the calling UI Dispatcher.</summary>
    public MediaPlayerElementPlaybackSessionAdapter(MediaPlayerElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        _dispatcherQueue = element.DispatcherQueue;
        _player = element.MediaPlayer ?? throw new InvalidOperationException("The MediaPlayerElement does not expose a MediaPlayer.");
        _playbackSession = _player.PlaybackSession;
        _currentSnapshot = CreateUnavailableSnapshot();

        _playbackStateChangedHandler = (_, _) => RequestSnapshotPublication();
        _positionChangedHandler = (_, _) => RequestSnapshotPublication();
        _mediaOpenedHandler = (_, _) =>
        {
            _isEnded = false;
            _lastErrorCode = MediaPlaybackErrorCode.None;
            RequestSnapshotPublication();
        };
        _mediaEndedHandler = (_, _) =>
        {
            _isEnded = true;
            RequestSnapshotPublication();
        };
        _mediaFailedHandler = (_, _) =>
        {
            _lastErrorCode = MediaPlaybackErrorCode.EngineFailure;
            RequestSnapshotPublication();
        };

        _playbackSession.PlaybackStateChanged += _playbackStateChangedHandler;
        _playbackSession.PositionChanged += _positionChangedHandler;
        _player.MediaOpened += _mediaOpenedHandler;
        _player.MediaEnded += _mediaEndedHandler;
        _player.MediaFailed += _mediaFailedHandler;
        PublishSnapshot();
    }

    /// <inheritdoc />
    public MediaPlaybackSnapshot CurrentSnapshot => Volatile.Read(ref _currentSnapshot);

    /// <inheritdoc />
    public event EventHandler<MediaPlaybackSessionChangedEventArgs>? SnapshotChanged;

    /// <inheritdoc />
    public Task<MediaPlaybackCommandResult> PlayAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(MediaPlaybackCapabilities.Play, () => _player.Play(), cancellationToken);

    /// <inheritdoc />
    public Task<MediaPlaybackCommandResult> PauseAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(MediaPlaybackCapabilities.Pause, () => _player.Pause(), cancellationToken);

    /// <inheritdoc />
    public Task<MediaPlaybackCommandResult> StopAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(MediaPlaybackCapabilities.Stop, () =>
        {
            // MediaPlayer has no independent Stop primitive. Its documented stop
            // equivalent is pause plus a return to the start when seeking is available.
            _player.Pause();
            if (_playbackSession.CanSeek)
            {
                _playbackSession.Position = TimeSpan.Zero;
            }
            _isEnded = false;
        }, cancellationToken);

    /// <inheritdoc />
    public Task<MediaPlaybackCommandResult> SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
    {
        if (position < TimeSpan.Zero)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument));
        }

        return ExecuteAsync(MediaPlaybackCapabilities.Seek, () => _playbackSession.Position = position, cancellationToken);
    }

    /// <inheritdoc />
    public Task<MediaPlaybackCommandResult> SetVolumeAsync(double volume, CancellationToken cancellationToken = default)
    {
        if (!double.IsFinite(volume) || volume is < 0 or > 1)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument));
        }

        return ExecuteAsync(MediaPlaybackCapabilities.Volume, () => _player.Volume = volume, cancellationToken);
    }

    /// <inheritdoc />
    public Task<MediaPlaybackCommandResult> SetPlaybackRateAsync(double playbackRate, CancellationToken cancellationToken = default)
    {
        if (!double.IsFinite(playbackRate) || playbackRate <= 0)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument));
        }

        return ExecuteAsync(MediaPlaybackCapabilities.PlaybackRate, () => _playbackSession.PlaybackRate = playbackRate, cancellationToken);
    }

    /// <summary>
    /// Stops forwarding native events. Disposal does not dispose the host-owned
    /// <see cref="MediaPlayerElement"/> or its underlying <see cref="MediaPlayer"/>.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        Interlocked.Increment(ref _generation);
        if (_dispatcherQueue.HasThreadAccess)
        {
            DetachNativeEvents();
        }
        else
        {
            _dispatcherQueue.TryEnqueue(DetachNativeEvents);
        }
    }

    private Task<MediaPlaybackCommandResult> ExecuteAsync(MediaPlaybackCapabilities capability, Action command, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Cancelled());
        }

        if (_isDisposed)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Cancelled());
        }

        if (!CurrentSnapshot.Capabilities.HasFlag(capability))
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected());
        }

        var completion = new TaskCompletionSource<MediaPlaybackCommandResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var generation = Volatile.Read(ref _generation);
        void ExecuteOnDispatcher()
        {
            if (_isDisposed || generation != Volatile.Read(ref _generation) || cancellationToken.IsCancellationRequested)
            {
                completion.TrySetResult(MediaPlaybackCommandResult.Cancelled());
                return;
            }

            try
            {
                command();
                PublishSnapshot();
                completion.TrySetResult(MediaPlaybackCommandResult.Success);
            }
            catch (Exception)
            {
                _lastErrorCode = MediaPlaybackErrorCode.EngineFailure;
                PublishSnapshot();
                completion.TrySetResult(MediaPlaybackCommandResult.Failed(MediaPlaybackErrorCode.EngineFailure));
            }
        }

        if (_dispatcherQueue.HasThreadAccess)
        {
            ExecuteOnDispatcher();
        }
        else if (!_dispatcherQueue.TryEnqueue(ExecuteOnDispatcher))
        {
            completion.TrySetResult(MediaPlaybackCommandResult.Failed(MediaPlaybackErrorCode.DispatcherUnavailable));
        }

        return completion.Task;
    }

    private void RequestSnapshotPublication()
    {
        var generation = Volatile.Read(ref _generation);
        void PublishIfCurrent()
        {
            if (!_isDisposed && generation == Volatile.Read(ref _generation))
            {
                PublishSnapshot();
            }
        }

        if (_dispatcherQueue.HasThreadAccess)
        {
            PublishIfCurrent();
        }
        else
        {
            _dispatcherQueue.TryEnqueue(PublishIfCurrent);
        }
    }

    private void PublishSnapshot()
    {
        if (_isDisposed)
        {
            return;
        }

        MediaPlaybackSnapshot snapshot;
        try
        {
            snapshot = CreateSnapshot();
        }
        catch (Exception)
        {
            _lastErrorCode = MediaPlaybackErrorCode.EngineFailure;
            snapshot = CreateUnavailableSnapshot(WinUI3.Senior.Core.MediaPlaybackState.Failed, _lastErrorCode);
        }

        Volatile.Write(ref _currentSnapshot, snapshot);
        SnapshotChanged?.Invoke(this, new MediaPlaybackSessionChangedEventArgs(snapshot));
    }

    private MediaPlaybackSnapshot CreateSnapshot()
    {
        var duration = _playbackSession.NaturalDuration;
        var seekableRange = duration > TimeSpan.Zero
            ? new MediaPlaybackTimeRange(TimeSpan.Zero, duration)
            : new MediaPlaybackTimeRange(TimeSpan.Zero, TimeSpan.Zero);
        var capabilities = MediaPlaybackCapabilities.Play | MediaPlaybackCapabilities.Stop | MediaPlaybackCapabilities.Volume | MediaPlaybackCapabilities.PlaybackRate;
        if (_playbackSession.CanPause)
        {
            capabilities |= MediaPlaybackCapabilities.Pause;
        }

        if (_playbackSession.CanSeek)
        {
            capabilities |= MediaPlaybackCapabilities.Seek;
        }

        var state = _lastErrorCode != MediaPlaybackErrorCode.None
            ? WinUI3.Senior.Core.MediaPlaybackState.Failed
            : _isEnded ? WinUI3.Senior.Core.MediaPlaybackState.Ended : MapState(_playbackSession.PlaybackState);
        return new MediaPlaybackSnapshot(
            _sessionId,
            Interlocked.Increment(ref _revision),
            state,
            duration > TimeSpan.Zero ? MediaPlaybackMode.VideoOnDemand : MediaPlaybackMode.Unknown,
            capabilities,
            _playbackSession.Position,
            seekableRange,
            [],
            _player.Volume,
            _playbackSession.PlaybackRate,
            _lastErrorCode);
    }

    private MediaPlaybackSnapshot CreateUnavailableSnapshot(
        ContractPlaybackState state = ContractPlaybackState.Unavailable,
        MediaPlaybackErrorCode errorCode = MediaPlaybackErrorCode.None) =>
        new(
            _sessionId,
            Interlocked.Increment(ref _revision),
            state,
            MediaPlaybackMode.Unknown,
            MediaPlaybackCapabilities.None,
            TimeSpan.Zero,
            new MediaPlaybackTimeRange(TimeSpan.Zero, TimeSpan.Zero),
            [],
            0,
            1,
            errorCode);

    private static ContractPlaybackState MapState(NativePlaybackState state) => state switch
    {
        NativePlaybackState.Opening => ContractPlaybackState.Loading,
        NativePlaybackState.Buffering => ContractPlaybackState.Buffering,
        NativePlaybackState.Playing => ContractPlaybackState.Playing,
        NativePlaybackState.Paused => ContractPlaybackState.Paused,
        _ => ContractPlaybackState.Ready,
    };

    private void DetachNativeEvents()
    {
        _playbackSession.PlaybackStateChanged -= _playbackStateChangedHandler;
        _playbackSession.PositionChanged -= _positionChangedHandler;
        _player.MediaOpened -= _mediaOpenedHandler;
        _player.MediaEnded -= _mediaEndedHandler;
        _player.MediaFailed -= _mediaFailedHandler;
    }
}
