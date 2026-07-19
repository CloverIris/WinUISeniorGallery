using Microsoft.UI.Dispatching;
using WinUI3.Senior.Core;

namespace WinUI3_Senior_Gallery;

/// <summary>Deterministic, in-memory playback session used only by Gallery laboratories.</summary>
internal sealed class MediaDemoSession : IMediaPlaybackSession, IDisposable
{
    private readonly DispatcherQueueTimer _timer;
    private readonly PlaybackSessionId _sessionId = new($"gallery-{Guid.NewGuid():N}");
    private MediaPlaybackMode _mode;
    private MediaPlaybackSnapshot _snapshot;
    private bool _disposed;

    public MediaDemoSession(MediaPlaybackMode mode = MediaPlaybackMode.VideoOnDemand)
    {
        _mode = mode;
        var range = mode == MediaPlaybackMode.LiveDvr
            ? new MediaPlaybackTimeRange(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(20))
            : new MediaPlaybackTimeRange(TimeSpan.Zero, TimeSpan.FromMinutes(4));
        _snapshot = CreateSnapshot(MediaPlaybackState.Ready, range.Start, range);
        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(250);
        _timer.Tick += (_, _) => Tick();
    }

    public MediaPlaybackSnapshot CurrentSnapshot => _snapshot;

    public event EventHandler<MediaPlaybackSessionChangedEventArgs>? SnapshotChanged;

    public Task<MediaPlaybackCommandResult> PlayAsync(CancellationToken cancellationToken = default) =>
        UpdateAsync(MediaPlaybackState.Playing, _snapshot.Position, cancellationToken);

    public Task<MediaPlaybackCommandResult> PauseAsync(CancellationToken cancellationToken = default) =>
        UpdateAsync(MediaPlaybackState.Paused, _snapshot.Position, cancellationToken);

    public Task<MediaPlaybackCommandResult> StopAsync(CancellationToken cancellationToken = default) =>
        UpdateAsync(MediaPlaybackState.Ready, _snapshot.SeekableRange.Start, cancellationToken);

    public Task<MediaPlaybackCommandResult> SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
    {
        if (_mode == MediaPlaybackMode.Live || !_snapshot.Capabilities.HasFlag(MediaPlaybackCapabilities.Seek))
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected());
        }

        return UpdateAsync(_snapshot.State, _snapshot.SeekableRange.Clamp(position), cancellationToken);
    }

    public Task<MediaPlaybackCommandResult> SetVolumeAsync(double volume, CancellationToken cancellationToken = default)
    {
        if (!double.IsFinite(volume) || volume is < 0 or > 1)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument));
        }

        return UpdateSnapshotAsync(CreateSnapshot(_snapshot.State, _snapshot.Position, _snapshot.SeekableRange, volume: volume, playbackRate: _snapshot.PlaybackRate), cancellationToken);
    }

    public Task<MediaPlaybackCommandResult> SetPlaybackRateAsync(double playbackRate, CancellationToken cancellationToken = default)
    {
        if (!double.IsFinite(playbackRate) || playbackRate <= 0)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Rejected(MediaPlaybackErrorCode.InvalidArgument));
        }

        return UpdateSnapshotAsync(CreateSnapshot(_snapshot.State, _snapshot.Position, _snapshot.SeekableRange, volume: _snapshot.Volume, playbackRate: playbackRate), cancellationToken);
    }

    public void SetMode(MediaPlaybackMode mode)
    {
        _mode = mode;
        var range = mode == MediaPlaybackMode.LiveDvr
            ? new MediaPlaybackTimeRange(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(20))
            : new MediaPlaybackTimeRange(TimeSpan.Zero, TimeSpan.FromMinutes(4));
        _snapshot = CreateSnapshot(MediaPlaybackState.Ready, range.Start, range, mode);
        Publish();
    }

    public void Dispose()
    {
        _disposed = true;
        _timer.Stop();
    }

    private Task<MediaPlaybackCommandResult> UpdateAsync(MediaPlaybackState state, TimeSpan position, CancellationToken cancellationToken)
    {
        var snapshot = CreateSnapshot(state, position, _snapshot.SeekableRange, volume: _snapshot.Volume, playbackRate: _snapshot.PlaybackRate);
        return UpdateSnapshotAsync(snapshot, cancellationToken);
    }

    private Task<MediaPlaybackCommandResult> UpdateSnapshotAsync(MediaPlaybackSnapshot snapshot, CancellationToken cancellationToken)
    {
        if (_disposed || cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(MediaPlaybackCommandResult.Cancelled());
        }

        _snapshot = snapshot;
        if (snapshot.State == MediaPlaybackState.Playing)
        {
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }

        Publish();
        return Task.FromResult(MediaPlaybackCommandResult.Success);
    }

    private void Tick()
    {
        if (_disposed || _snapshot.State != MediaPlaybackState.Playing)
        {
            return;
        }

        var range = _snapshot.SeekableRange;
        var position = _snapshot.Position + TimeSpan.FromMilliseconds(250 * _snapshot.PlaybackRate);
        if (_mode == MediaPlaybackMode.LiveDvr)
        {
            range = new MediaPlaybackTimeRange(range.Start + TimeSpan.FromMilliseconds(250), range.End + TimeSpan.FromMilliseconds(250));
        }

        if (_mode != MediaPlaybackMode.LiveDvr && position >= range.End)
        {
            _snapshot = CreateSnapshot(MediaPlaybackState.Ended, range.End, range, volume: _snapshot.Volume, playbackRate: _snapshot.PlaybackRate);
            _timer.Stop();
        }
        else
        {
            _snapshot = CreateSnapshot(_snapshot.State, range.Clamp(position), range, volume: _snapshot.Volume, playbackRate: _snapshot.PlaybackRate);
        }

        Publish();
    }

    private MediaPlaybackSnapshot CreateSnapshot(
        MediaPlaybackState state,
        TimeSpan position,
        MediaPlaybackTimeRange range,
        MediaPlaybackMode? mode = null,
        double? volume = null,
        double? playbackRate = null) =>
        new(
            _sessionId,
            _snapshot is null ? 0 : _snapshot.Revision + 1,
            state,
            mode ?? _mode,
            (mode ?? _mode) == MediaPlaybackMode.Live
                ? MediaPlaybackCapabilities.Play | MediaPlaybackCapabilities.Pause | MediaPlaybackCapabilities.Stop | MediaPlaybackCapabilities.Volume | MediaPlaybackCapabilities.PlaybackRate
                : MediaPlaybackCapabilities.Play | MediaPlaybackCapabilities.Pause | MediaPlaybackCapabilities.Stop | MediaPlaybackCapabilities.Seek | MediaPlaybackCapabilities.Volume | MediaPlaybackCapabilities.PlaybackRate,
            position,
            range,
            [new MediaPlaybackTimeRange(range.Start, range.Start + TimeSpan.FromTicks((range.End - range.Start).Ticks / 2))],
            volume ?? 0.75,
            playbackRate ?? 1.0);

    private void Publish() => SnapshotChanged?.Invoke(this, new MediaPlaybackSessionChangedEventArgs(_snapshot));
}
