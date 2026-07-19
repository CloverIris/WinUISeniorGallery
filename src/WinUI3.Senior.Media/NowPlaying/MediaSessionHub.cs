using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

/// <summary>Describes a session binding change for one explicit host key.</summary>
public sealed class MediaSessionChangedEventArgs(
    string hostKey,
    IMediaPlaybackSession? previousSession,
    IMediaPlaybackSession? currentSession) : EventArgs
{
    public string HostKey { get; } = hostKey;
    public IMediaPlaybackSession? PreviousSession { get; } = previousSession;
    public IMediaPlaybackSession? CurrentSession { get; } = currentSession;
}

/// <summary>
/// Keeps playback sessions scoped to explicit hosts or windows. It intentionally has no
/// "active window" fallback, preventing commands from leaking to an unrelated surface.
/// </summary>
public sealed class MediaSessionHub : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<string, IMediaPlaybackSession> _sessions = new(StringComparer.Ordinal);
    private bool _disposed;

    public event EventHandler<MediaSessionChangedEventArgs>? SessionChanged;

    public IReadOnlyCollection<string> HostKeys
    {
        get
        {
            lock (_gate) return _sessions.Keys.ToArray();
        }
    }

    public bool TryGet(string hostKey, out IMediaPlaybackSession? session)
    {
        ValidateHostKey(hostKey);
        lock (_gate) return _sessions.TryGetValue(hostKey, out session);
    }

    public bool Register(string hostKey, IMediaPlaybackSession session, bool replaceExisting = true)
    {
        ValidateHostKey(hostKey);
        ArgumentNullException.ThrowIfNull(session);
        ThrowIfDisposed();

        IMediaPlaybackSession? previous;
        lock (_gate)
        {
            if (_sessions.TryGetValue(hostKey, out previous) && !replaceExisting) return false;
            if (ReferenceEquals(previous, session)) return true;
            _sessions[hostKey] = session;
        }

        SessionChanged?.Invoke(this, new MediaSessionChangedEventArgs(hostKey, previous, session));
        return true;
    }

    public bool Unregister(string hostKey, IMediaPlaybackSession? expectedSession = null)
    {
        ValidateHostKey(hostKey);
        ThrowIfDisposed();
        IMediaPlaybackSession? previous;
        lock (_gate)
        {
            if (!_sessions.TryGetValue(hostKey, out previous)) return false;
            if (expectedSession is not null && !ReferenceEquals(expectedSession, previous)) return false;
            _sessions.Remove(hostKey);
        }

        SessionChanged?.Invoke(this, new MediaSessionChangedEventArgs(hostKey, previous, null));
        return true;
    }

    /// <summary>Replaces one host binding atomically and reports the old session to the host.</summary>
    public bool TryReplace(string hostKey, IMediaPlaybackSession? session)
    {
        ValidateHostKey(hostKey);
        ThrowIfDisposed();
        if (session is null) return Unregister(hostKey);
        return Register(hostKey, session, replaceExisting: true);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        KeyValuePair<string, IMediaPlaybackSession>[] bindings;
        lock (_gate)
        {
            bindings = _sessions.ToArray();
            _sessions.Clear();
        }

        foreach (var binding in bindings) SessionChanged?.Invoke(this, new MediaSessionChangedEventArgs(binding.Key, binding.Value, null));
        SessionChanged = null;
    }

    private static void ValidateHostKey(string hostKey)
    {
        if (string.IsNullOrWhiteSpace(hostKey)) throw new ArgumentException("A non-empty host key is required.", nameof(hostKey));
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(MediaSessionHub));
    }
}
