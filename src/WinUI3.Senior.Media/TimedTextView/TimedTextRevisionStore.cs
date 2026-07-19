namespace WinUI3.Senior.Media;

/// <summary>Describes why a timed-text document was or was not accepted.</summary>
public enum TimedTextUpdateStatus
{
    Accepted,
    RejectedStale,
    RejectedIdentity,
    RejectedInvalid,
}

public sealed record TimedTextUpdateResult(TimedTextUpdateStatus Status, TimedTextDocument? Document)
{
    public bool IsAccepted => Status == TimedTextUpdateStatus.Accepted && Document is not null;
}

/// <summary>
/// Applies immutable timed-text snapshots monotonically. The store is provider-neutral and can
/// be used by ASR/translation adapters later without allowing stale callbacks to regress a view.
/// </summary>
public sealed class TimedTextRevisionStore
{
    private readonly object _gate = new();
    private TimedTextDocument? _current;

    public TimedTextDocument? Current
    {
        get { lock (_gate) return _current; }
    }

    public event EventHandler<TimedTextDocumentChangedEventArgs>? DocumentChanged;

    public TimedTextUpdateResult Apply(TimedTextDocument? candidate)
    {
        var normalized = TimedTextNormalizer.Normalize(candidate);
        if (normalized is null) return new TimedTextUpdateResult(TimedTextUpdateStatus.RejectedInvalid, Current);

        lock (_gate)
        {
            if (_current is not null)
            {
                if (!StringComparer.Ordinal.Equals(_current.Id, normalized.Id))
                    return new TimedTextUpdateResult(TimedTextUpdateStatus.RejectedIdentity, _current);
                if (normalized.Revision <= _current.Revision)
                    return new TimedTextUpdateResult(TimedTextUpdateStatus.RejectedStale, _current);
            }

            _current = normalized;
        }

        DocumentChanged?.Invoke(this, new TimedTextDocumentChangedEventArgs(normalized));
        return new TimedTextUpdateResult(TimedTextUpdateStatus.Accepted, normalized);
    }

    /// <summary>Starts a new media document explicitly, resetting the revision fence.</summary>
    public TimedTextUpdateResult StartNew(TimedTextDocument? candidate)
    {
        var normalized = TimedTextNormalizer.Normalize(candidate);
        if (normalized is null) return new TimedTextUpdateResult(TimedTextUpdateStatus.RejectedInvalid, Current);
        lock (_gate) _current = normalized;
        DocumentChanged?.Invoke(this, new TimedTextDocumentChangedEventArgs(normalized));
        return new TimedTextUpdateResult(TimedTextUpdateStatus.Accepted, normalized);
    }

    public void Clear()
    {
        lock (_gate)
        {
            if (_current is null) return;
            _current = null;
        }
        DocumentChanged?.Invoke(this, new TimedTextDocumentChangedEventArgs(null));
    }
}

public sealed class TimedTextDocumentChangedEventArgs(TimedTextDocument? document) : EventArgs
{
    public TimedTextDocument? Document { get; } = document;
}

