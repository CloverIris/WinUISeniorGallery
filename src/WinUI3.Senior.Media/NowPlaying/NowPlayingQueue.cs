using System.Collections.Immutable;

namespace WinUI3.Senior.Media;

/// <summary>
/// Thread-safe queue model for media surfaces. It owns ordering only; it never opens files,
/// creates playback sessions, or performs network work.
/// </summary>
public sealed class NowPlayingQueue
{
    private readonly object _gate = new();
    private ImmutableArray<NowPlayingMediaItem> _items;
    private int _selectedIndex = -1;
    private long _revision;

    public NowPlayingQueue(IEnumerable<NowPlayingMediaItem>? items = null)
    {
        _items = (items ?? Enumerable.Empty<NowPlayingMediaItem>()).ToImmutableArray();
        EnsureUniqueIds(_items);
        _selectedIndex = _items.Length == 0 ? -1 : 0;
    }

    public event EventHandler? Changed;

    public ImmutableArray<NowPlayingMediaItem> Items
    {
        get { lock (_gate) return _items; }
    }

    public int SelectedIndex
    {
        get { lock (_gate) return _selectedIndex; }
    }

    public NowPlayingMediaItem? SelectedItem
    {
        get
        {
            lock (_gate) return _selectedIndex >= 0 && _selectedIndex < _items.Length ? _items[_selectedIndex] : null;
        }
    }

    public long Revision
    {
        get { lock (_gate) return _revision; }
    }

    public bool ContainsId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        lock (_gate) return _items.Any(item => string.Equals(item.Id, id, StringComparison.Ordinal));
    }

    public int FindIndex(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return -1;
        lock (_gate)
        {
            for (var i = 0; i < _items.Length; i++)
                if (string.Equals(_items[i].Id, id, StringComparison.Ordinal)) return i;
            return -1;
        }
    }

    public void Enqueue(NowPlayingMediaItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        lock (_gate)
        {
            if (_items.Any(existing => string.Equals(existing.Id, item.Id, StringComparison.Ordinal)))
                throw new ArgumentException($"A queue item with id '{item.Id}' already exists.", nameof(item));
            _items = _items.Add(item);
            if (_selectedIndex < 0) _selectedIndex = 0;
            _revision++;
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void EnqueueRange(IEnumerable<NowPlayingMediaItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var additions = items.ToImmutableArray();
        if (additions.IsDefaultOrEmpty) return;
        EnsureUniqueIds(additions);

        lock (_gate)
        {
            if (additions.Any(item => _items.Any(existing => string.Equals(existing.Id, item.Id, StringComparison.Ordinal))))
                throw new ArgumentException("Queue item ids must be unique across the queue.", nameof(items));
            _items = _items.AddRange(additions);
            if (_selectedIndex < 0) _selectedIndex = 0;
            _revision++;
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public bool TrySelect(int index)
    {
        lock (_gate)
        {
            if (index < 0 || index >= _items.Length) return false;
            if (_selectedIndex == index) return true;
            _selectedIndex = index;
            _revision++;
        }
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool TrySelect(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        lock (_gate)
        {
            var index = -1;
            for (var i = 0; i < _items.Length; i++)
            {
                if (string.Equals(_items[i].Id, id, StringComparison.Ordinal))
                {
                    index = i;
                    break;
                }
            }
            if (index < 0) return false;
            if (_selectedIndex == index) return true;
            _selectedIndex = index;
            _revision++;
        }
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool TryInsert(int index, NowPlayingMediaItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        lock (_gate)
        {
            if (index < 0 || index > _items.Length) return false;
            if (_items.Any(existing => string.Equals(existing.Id, item.Id, StringComparison.Ordinal))) return false;
            _items = _items.Insert(index, item);
            if (_selectedIndex < 0) _selectedIndex = 0;
            else if (index <= _selectedIndex) _selectedIndex++;
            _revision++;
        }
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool TryReplace(int index, NowPlayingMediaItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        lock (_gate)
        {
            if (index < 0 || index >= _items.Length) return false;
            if (_items.Where((_, candidateIndex) => candidateIndex != index).Any(existing => string.Equals(existing.Id, item.Id, StringComparison.Ordinal))) return false;
            var builder = _items.ToBuilder();
            builder[index] = item;
            _items = builder.ToImmutable();
            _revision++;
        }
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool TryRemove(string id, out NowPlayingMediaItem? removed)
    {
        var index = FindIndex(id);
        return index >= 0 && TryRemove(index, out removed);
    }

    public bool TryRemove(int index, out NowPlayingMediaItem? removed)
    {
        lock (_gate)
        {
            if (index < 0 || index >= _items.Length)
            {
                removed = null;
                return false;
            }

            removed = _items[index];
            _items = _items.RemoveAt(index);
            _selectedIndex = _items.Length == 0
                ? -1
                : _selectedIndex > index ? _selectedIndex - 1 : Math.Min(_selectedIndex, _items.Length - 1);
            _revision++;
        }
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool TryMove(int oldIndex, int newIndex)
    {
        lock (_gate)
        {
            if (oldIndex < 0 || oldIndex >= _items.Length || newIndex < 0 || newIndex >= _items.Length) return false;
            if (oldIndex == newIndex) return true;

            var item = _items[oldIndex];
            var builder = _items.ToBuilder();
            builder.RemoveAt(oldIndex);
            builder.Insert(newIndex, item);
            _items = builder.ToImmutable();

            if (_selectedIndex == oldIndex) _selectedIndex = newIndex;
            else if (oldIndex < _selectedIndex && newIndex >= _selectedIndex) _selectedIndex--;
            else if (oldIndex > _selectedIndex && newIndex <= _selectedIndex) _selectedIndex++;
            _revision++;
        }
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void Clear()
    {
        lock (_gate)
        {
            if (_items.IsEmpty && _selectedIndex < 0) return;
            _items = ImmutableArray<NowPlayingMediaItem>.Empty;
            _selectedIndex = -1;
            _revision++;
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Returns the next queue index while respecting the repeat mode.</summary>
    public int GetNextIndex(NowPlayingRepeatMode repeatMode)
    {
        lock (_gate)
        {
            if (_items.IsEmpty || _selectedIndex < 0) return -1;
            if (repeatMode == NowPlayingRepeatMode.One) return _selectedIndex;
            if (_selectedIndex < _items.Length - 1) return _selectedIndex + 1;
            return repeatMode == NowPlayingRepeatMode.All ? 0 : -1;
        }
    }

    /// <summary>Returns the previous queue index while respecting the repeat mode.</summary>
    public int GetPreviousIndex(NowPlayingRepeatMode repeatMode)
    {
        lock (_gate)
        {
            if (_items.IsEmpty || _selectedIndex < 0) return -1;
            if (_selectedIndex > 0) return _selectedIndex - 1;
            return repeatMode == NowPlayingRepeatMode.All ? _items.Length - 1 : 0;
        }
    }

    private static void EnsureUniqueIds(IEnumerable<NowPlayingMediaItem> items)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var item in items)
        {
            if (!seen.Add(item.Id))
                throw new ArgumentException($"Queue item ids must be unique; duplicate '{item.Id}'.", nameof(items));
        }
    }
}
