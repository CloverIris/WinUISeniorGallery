using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Controls;

/// <summary>Display sizes used by a DynamicTile host when it maps tiles to a responsive grid.</summary>
public enum DynamicTileSize
{
    Small,
    Medium,
    Wide,
    Large
}

public enum DynamicTileTransition
{
    None,
    Flip,
    Slide,
    Fade
}

/// <summary>
/// A bit mask instead of one boolean so hosts can pause a tile for several independent reasons.
/// A tile resumes only after every active reason has been cleared.
/// </summary>
[Flags]
public enum DynamicTilePauseReason
{
    None = 0,
    NotVisible = 1,
    WindowInactive = 2,
    UserInteraction = 4,
    ReducedMotion = 8,
    Host = 16
}

public sealed record DynamicTileFrame
{
    public DynamicTileFrame(string id, object? content, string? title = null, string? automationName = null, TimeSpan? duration = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("A frame id is required.", nameof(id));
        if (duration is { } value && (value < TimeSpan.FromMilliseconds(100) || value > TimeSpan.FromMinutes(10)))
            throw new ArgumentOutOfRangeException(nameof(duration), "A frame duration must be between 100 ms and 10 minutes.");

        Id = id;
        Content = content;
        Title = title;
        AutomationName = automationName;
        Duration = duration;
    }

    public string Id { get; }
    public object? Content { get; }
    public string? Title { get; }
    public string? AutomationName { get; }
    public TimeSpan? Duration { get; }
}

public sealed class DynamicTileFrameChangedEventArgs : EventArgs
{
    public DynamicTileFrameChangedEventArgs(DynamicTileFrame? previousFrame, DynamicTileFrame? currentFrame, int index, bool automatic)
    {
        PreviousFrame = previousFrame;
        CurrentFrame = currentFrame;
        Index = index;
        IsAutomatic = automatic;
    }

    public DynamicTileFrame? PreviousFrame { get; }
    public DynamicTileFrame? CurrentFrame { get; }
    public int Index { get; }
    public bool IsAutomatic { get; }
}

/// <summary>
/// A host-owned, local-data tile rotator. It never creates a window or fetches data by itself.
/// The optional <see cref="RefreshProvider"/> is supplied by the host and can be cancelled on unload.
/// </summary>
public sealed class DynamicTile : ContentControl, IDisposable
{
    private readonly object _gate = new();
    private DispatcherQueueTimer? _timer;
    private CancellationTokenSource? _refreshCts;
    private int _refreshRevision;
    private bool _disposed;
    private DynamicTilePauseReason _pauseReasons;
    private bool _isLoaded;

    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(nameof(Size), typeof(DynamicTileSize), typeof(DynamicTile), new PropertyMetadata(DynamicTileSize.Medium));
    public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(string), typeof(DynamicTile), new PropertyMetadata(null));
    public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(nameof(Transition), typeof(DynamicTileTransition), typeof(DynamicTile), new PropertyMetadata(DynamicTileTransition.Fade));
    public static readonly DependencyProperty CycleIntervalProperty = DependencyProperty.Register(nameof(CycleInterval), typeof(TimeSpan), typeof(DynamicTile), new PropertyMetadata(TimeSpan.FromSeconds(8), OnTimingPropertyChanged));
    public static readonly DependencyProperty CurrentFrameIndexProperty = DependencyProperty.Register(nameof(CurrentFrameIndex), typeof(int), typeof(DynamicTile), new PropertyMetadata(-1));
    public static readonly DependencyProperty CurrentFrameProperty = DependencyProperty.Register(nameof(CurrentFrame), typeof(DynamicTileFrame), typeof(DynamicTile), new PropertyMetadata(null));
    public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register(nameof(IsPinned), typeof(bool), typeof(DynamicTile), new PropertyMetadata(true));
    public static readonly DependencyProperty IsAutoPlayEnabledProperty = DependencyProperty.Register(nameof(IsAutoPlayEnabled), typeof(bool), typeof(DynamicTile), new PropertyMetadata(true, OnTimingPropertyChanged));
    public static readonly DependencyProperty MaxFrameCountProperty = DependencyProperty.Register(nameof(MaxFrameCount), typeof(int), typeof(DynamicTile), new PropertyMetadata(3, OnMaxFrameCountChanged));
    public static readonly DependencyProperty IsReducedMotionProperty = DependencyProperty.Register(nameof(IsReducedMotion), typeof(bool), typeof(DynamicTile), new PropertyMetadata(false, OnReducedMotionChanged));
    public static readonly DependencyProperty IsHostVisibleProperty = DependencyProperty.Register(nameof(IsHostVisible), typeof(bool), typeof(DynamicTile), new PropertyMetadata(true, OnHostStateChanged));
    public static readonly DependencyProperty IsHostWindowActiveProperty = DependencyProperty.Register(nameof(IsHostWindowActive), typeof(bool), typeof(DynamicTile), new PropertyMetadata(true, OnHostStateChanged));

    public DynamicTile()
    {
        DefaultStyleKey = typeof(DynamicTile);
        Frames = new ObservableCollection<DynamicTileFrame>();
        Frames.CollectionChanged += OnFramesChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public ObservableCollection<DynamicTileFrame> Frames { get; }
    public string? Id { get => (string?)GetValue(IdProperty); set => SetValue(IdProperty, value); }
    public DynamicTileSize Size { get => (DynamicTileSize)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }
    public DynamicTileTransition Transition { get => (DynamicTileTransition)GetValue(TransitionProperty); set => SetValue(TransitionProperty, value); }
    public TimeSpan CycleInterval { get => (TimeSpan)GetValue(CycleIntervalProperty); set => SetValue(CycleIntervalProperty, value); }
    public int CurrentFrameIndex { get => (int)GetValue(CurrentFrameIndexProperty); private set => SetValue(CurrentFrameIndexProperty, value); }
    public DynamicTileFrame? CurrentFrame { get => (DynamicTileFrame?)GetValue(CurrentFrameProperty); private set => SetValue(CurrentFrameProperty, value); }
    public bool IsPinned { get => (bool)GetValue(IsPinnedProperty); set => SetValue(IsPinnedProperty, value); }
    public bool IsAutoPlayEnabled { get => (bool)GetValue(IsAutoPlayEnabledProperty); set => SetValue(IsAutoPlayEnabledProperty, value); }
    public int MaxFrameCount { get => (int)GetValue(MaxFrameCountProperty); set => SetValue(MaxFrameCountProperty, Math.Max(0, value)); }
    public bool IsReducedMotion { get => (bool)GetValue(IsReducedMotionProperty); set => SetValue(IsReducedMotionProperty, value); }
    public bool IsHostVisible { get => (bool)GetValue(IsHostVisibleProperty); set => SetValue(IsHostVisibleProperty, value); }
    public bool IsHostWindowActive { get => (bool)GetValue(IsHostWindowActiveProperty); set => SetValue(IsHostWindowActiveProperty, value); }
    public DynamicTilePauseReason PauseReasons => _pauseReasons;
    public Func<CancellationToken, ValueTask<IReadOnlyList<DynamicTileFrame>>>? RefreshProvider { get; set; }

    public event EventHandler<DynamicTileFrameChangedEventArgs>? FrameChanged;
    public event EventHandler? RefreshStarted;
    public event EventHandler<Exception>? RefreshFailed;
    public event EventHandler? RefreshCompleted;

    public void SetPauseReason(DynamicTilePauseReason reason, bool enabled)
    {
        if (reason == DynamicTilePauseReason.None) return;
        var old = _pauseReasons;
        _pauseReasons = enabled ? old | reason : old & ~reason;
        if (old == _pauseReasons) return;
        if (_pauseReasons != DynamicTilePauseReason.None) _timer?.Stop();
        else if (_isLoaded) RestartTimer();
    }

    public void Start() { if (!_disposed) { _isLoaded = true; RestartTimer(); } }
    public void Stop() => _timer?.Stop();

    public bool MoveTo(int index, bool automatic = false)
    {
        if (_disposed || index < 0 || index >= Frames.Count) return false;
        if (index == CurrentFrameIndex && CurrentFrame is not null) return true;
        var previous = CurrentFrame;
        CurrentFrameIndex = index;
        CurrentFrame = Frames[index];
        Content = CurrentFrame.Content;
        FrameChanged?.Invoke(this, new DynamicTileFrameChangedEventArgs(previous, CurrentFrame, index, automatic));
        RestartTimer();
        return true;
    }

    public bool Next(bool automatic = false)
    {
        if (Frames.Count == 0) return false;
        var index = CurrentFrameIndex < 0 ? 0 : (CurrentFrameIndex + 1) % Frames.Count;
        return MoveTo(index, automatic);
    }

    public bool Previous()
    {
        if (Frames.Count == 0) return false;
        var index = CurrentFrameIndex <= 0 ? Frames.Count - 1 : CurrentFrameIndex - 1;
        return MoveTo(index);
    }

    public async ValueTask<bool> RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed || RefreshProvider is null) return false;
        CancellationToken token;
        var revision = Interlocked.Increment(ref _refreshRevision);
        lock (_gate)
        {
            _refreshCts?.Cancel();
            _refreshCts?.Dispose();
            _refreshCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            token = _refreshCts.Token;
        }

        RefreshStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            var frames = await RefreshProvider(token);
            token.ThrowIfCancellationRequested();
            if (_disposed || revision != _refreshRevision) return false;
            ReplaceFrames(frames);
            RefreshCompleted?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            return false;
        }
        catch (Exception ex)
        {
            if (!_disposed && revision == _refreshRevision) RefreshFailed?.Invoke(this, ex);
            return false;
        }
    }

    public void ReplaceFrames(IEnumerable<DynamicTileFrame> frames)
    {
        ArgumentNullException.ThrowIfNull(frames);
        var snapshot = frames.ToArray();
        if (MaxFrameCount > 0 && snapshot.Length > MaxFrameCount) snapshot = snapshot.TakeLast(MaxFrameCount).ToArray();
        if (snapshot.Select(static frame => frame.Id).Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Frame ids must be unique within a tile.", nameof(frames));
        Frames.Clear();
        foreach (var frame in snapshot) Frames.Add(frame);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _timer?.Stop();
        _refreshCts?.Cancel();
        _refreshCts?.Dispose();
        Frames.CollectionChanged -= OnFramesChanged;
        Loaded -= OnLoaded;
        Unloaded -= OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) { _isLoaded = true; RestartTimer(); }
    private void OnUnloaded(object sender, RoutedEventArgs e) { _isLoaded = false; _timer?.Stop(); }
    private void OnFramesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (Frames.Count == 0)
        {
            CurrentFrameIndex = -1;
            CurrentFrame = null;
            Content = null;
            _timer?.Stop();
            return;
        }
        var index = CurrentFrameIndex < 0 ? 0 : Math.Min(CurrentFrameIndex, Frames.Count - 1);
        MoveTo(index);
    }

    private void RestartTimer()
    {
        _timer?.Stop();
        if (!_isLoaded || !IsAutoPlayEnabled || _pauseReasons != DynamicTilePauseReason.None || Frames.Count < 2 || _disposed) return;
        _timer ??= DispatcherQueue.GetForCurrentThread()?.CreateTimer();
        if (_timer is null) return;
        _timer.IsRepeating = false;
        _timer.Tick -= OnTimerTick;
        _timer.Tick += OnTimerTick;
        var duration = CurrentFrame?.Duration ?? CycleInterval;
        _timer.Interval = duration < TimeSpan.FromMilliseconds(100) ? TimeSpan.FromMilliseconds(100) : duration;
        _timer.Start();
    }

    private void OnTimerTick(DispatcherQueueTimer sender, object args) { if (_pauseReasons == DynamicTilePauseReason.None) Next(true); }
    private static void OnTimingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DynamicTile)d).RestartTimer();
    private static void OnHostStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var tile = (DynamicTile)d;
        tile.SetPauseReason(DynamicTilePauseReason.NotVisible, !tile.IsHostVisible);
        tile.SetPauseReason(DynamicTilePauseReason.WindowInactive, !tile.IsHostWindowActive);
    }

    private static void OnReducedMotionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var tile = (DynamicTile)d;
        tile.SetPauseReason(DynamicTilePauseReason.ReducedMotion, tile.IsReducedMotion);
    }

    private static void OnMaxFrameCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var tile = (DynamicTile)d;
        if (tile.MaxFrameCount > 0 && tile.Frames.Count > tile.MaxFrameCount)
            tile.ReplaceFrames(tile.Frames.TakeLast(tile.MaxFrameCount));
    }
}

/// <summary>
/// Coordinates several DynamicTile instances without taking ownership of their data source or
/// persistence. It provides deterministic ordering and broadcasts host pause state to each tile.
/// </summary>
public sealed class DynamicTileBoard : Control, IDisposable
{
    private bool _disposed;

    public static readonly DependencyProperty IsHostVisibleProperty = DependencyProperty.Register(nameof(IsHostVisible), typeof(bool), typeof(DynamicTileBoard), new PropertyMetadata(true, OnHostStateChanged));
    public static readonly DependencyProperty IsHostWindowActiveProperty = DependencyProperty.Register(nameof(IsHostWindowActive), typeof(bool), typeof(DynamicTileBoard), new PropertyMetadata(true, OnHostStateChanged));
    public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(nameof(IsEditMode), typeof(bool), typeof(DynamicTileBoard), new PropertyMetadata(false));

    public DynamicTileBoard()
    {
        DefaultStyleKey = typeof(DynamicTileBoard);
        Tiles = new ObservableCollection<DynamicTile>();
        Tiles.CollectionChanged += OnTilesChanged;
    }

    public ObservableCollection<DynamicTile> Tiles { get; }
    public bool IsHostVisible { get => (bool)GetValue(IsHostVisibleProperty); set => SetValue(IsHostVisibleProperty, value); }
    public bool IsHostWindowActive { get => (bool)GetValue(IsHostWindowActiveProperty); set => SetValue(IsHostWindowActiveProperty, value); }
    public bool IsEditMode { get => (bool)GetValue(IsEditModeProperty); set => SetValue(IsEditModeProperty, value); }

    public event EventHandler<DynamicTileFrameChangedEventArgs>? TileFrameChanged;
    public event EventHandler? OrderChanged;

    public bool MoveTile(string id, int destinationIndex)
    {
        if (_disposed || !IsEditMode) return false;
        var sourceIndex = IndexOf(id);
        if (sourceIndex < 0) return false;
        destinationIndex = Math.Clamp(destinationIndex, 0, Tiles.Count - 1);
        if (sourceIndex == destinationIndex) return true;
        var tile = Tiles[sourceIndex];
        Tiles.RemoveAt(sourceIndex);
        Tiles.Insert(destinationIndex, tile);
        OrderChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool SetPinned(string id, bool isPinned)
    {
        var index = IndexOf(id);
        if (index < 0) return false;
        Tiles[index].IsPinned = isPinned;
        return true;
    }

    public void SetPauseReason(DynamicTilePauseReason reason, bool enabled)
    {
        foreach (var tile in Tiles) tile.SetPauseReason(reason, enabled);
    }

    public async ValueTask<int> RefreshAllAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) return 0;
        var tasks = Tiles.Where(tile => tile.RefreshProvider is not null).Select(tile => tile.RefreshAsync(cancellationToken).AsTask()).ToArray();
        if (tasks.Length == 0) return 0;
        var results = await Task.WhenAll(tasks);
        return results.Count(static value => value);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var tile in Tiles)
        {
            tile.SetPauseReason(DynamicTilePauseReason.Host, true);
            tile.FrameChanged -= OnTileFrameChanged;
        }
        Tiles.CollectionChanged -= OnTilesChanged;
    }

    private int IndexOf(string id)
    {
        for (var index = 0; index < Tiles.Count; index++)
            if (string.Equals(Tiles[index].Id, id, StringComparison.Ordinal)) return index;
        return -1;
    }

    private void OnTilesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null) foreach (DynamicTile tile in e.OldItems) tile.FrameChanged -= OnTileFrameChanged;
        if (e.NewItems is not null)
            foreach (DynamicTile tile in e.NewItems)
            {
                tile.FrameChanged += OnTileFrameChanged;
                tile.IsHostVisible = IsHostVisible;
                tile.IsHostWindowActive = IsHostWindowActive;
            }
        OrderChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnTileFrameChanged(object? sender, DynamicTileFrameChangedEventArgs e) => TileFrameChanged?.Invoke(this, e);
    private static void OnHostStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var board = (DynamicTileBoard)d;
        foreach (var tile in board.Tiles)
        {
            tile.IsHostVisible = board.IsHostVisible;
            tile.IsHostWindowActive = board.IsHostWindowActive;
        }
    }
}
