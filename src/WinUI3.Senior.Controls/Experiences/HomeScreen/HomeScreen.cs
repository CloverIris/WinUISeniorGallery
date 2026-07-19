using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Controls;

public enum HomeScreenItemKind
{
    Application,
    RecentFile,
    Contact,
    Widget,
    Shortcut,
    Custom
}

public sealed record HomeScreenItem
{
    public HomeScreenItem(string id, string title, object? content = null, HomeScreenItemKind kind = HomeScreenItemKind.Custom,
        string? subtitle = null, string? group = null, bool isPinned = false, int order = 0, string? automationName = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("An item id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("An item title is required.", nameof(title));
        Id = id;
        Title = title;
        Content = content;
        Kind = kind;
        Subtitle = subtitle;
        Group = group;
        IsPinned = isPinned;
        Order = order;
        AutomationName = automationName;
    }

    public string Id { get; init; }
    public string Title { get; init; }
    public object? Content { get; init; }
    public HomeScreenItemKind Kind { get; init; }
    public string? Subtitle { get; init; }
    public string? Group { get; init; }
    public bool IsPinned { get; init; }
    public int Order { get; init; }
    public string? AutomationName { get; init; }
}

public class HomeScreenItemEventArgs : EventArgs
{
    public HomeScreenItemEventArgs(HomeScreenItem item, int index) { Item = item; Index = index; }
    public HomeScreenItem Item { get; }
    public int Index { get; }
}

public sealed record HomeScreenSection(string Id, string Title, IReadOnlyList<HomeScreenItem> Items, bool IsCollapsed = false)
{
    public int Count => Items.Count;
}

public sealed class HomeScreenPinChangedEventArgs : HomeScreenItemEventArgs
{
    public HomeScreenPinChangedEventArgs(HomeScreenItem item, int index, bool isPinned) : base(item, index) => IsPinned = isPinned;
    public bool IsPinned { get; }
}

/// <summary>
/// Local, host-owned Home screen model and control. Pin order and filtering stay in memory;
/// persistence, navigation and launch are deliberately delegated to the host.
/// </summary>
public sealed class HomeScreen : Control, IDisposable
{
    private readonly ReadOnlyObservableCollection<HomeScreenItem> _visibleItems;
    private readonly ReadOnlyObservableCollection<HomeScreenItem> _pinnedItems;
    private readonly ReadOnlyObservableCollection<HomeScreenSection> _visibleSections;
    private readonly ObservableCollection<HomeScreenItem> _visibleBacking = new();
    private readonly ObservableCollection<HomeScreenItem> _pinnedBacking = new();
    private readonly ObservableCollection<HomeScreenSection> _sectionBacking = new();
    private CancellationTokenSource? _refreshCts;
    private int _refreshRevision;
    private bool _disposed;

    public static readonly DependencyProperty SearchQueryProperty = DependencyProperty.Register(nameof(SearchQuery), typeof(string), typeof(HomeScreen), new PropertyMetadata(string.Empty, OnSearchQueryChanged));
    public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(nameof(IsEditMode), typeof(bool), typeof(HomeScreen), new PropertyMetadata(false));
    public static readonly DependencyProperty IsSearchEnabledProperty = DependencyProperty.Register(nameof(IsSearchEnabled), typeof(bool), typeof(HomeScreen), new PropertyMetadata(true, OnSearchQueryChanged));
    public static readonly DependencyProperty MaxPinnedItemsProperty = DependencyProperty.Register(nameof(MaxPinnedItems), typeof(int), typeof(HomeScreen), new PropertyMetadata(0));

    public HomeScreen()
    {
        DefaultStyleKey = typeof(HomeScreen);
        Items = new ObservableCollection<HomeScreenItem>();
        Items.CollectionChanged += OnItemsChanged;
        _visibleItems = new ReadOnlyObservableCollection<HomeScreenItem>(_visibleBacking);
        _pinnedItems = new ReadOnlyObservableCollection<HomeScreenItem>(_pinnedBacking);
        _visibleSections = new ReadOnlyObservableCollection<HomeScreenSection>(_sectionBacking);
        Loaded += (_, _) => RecomputeViews();
        Unloaded += (_, _) => CancelRefresh();
    }

    public ObservableCollection<HomeScreenItem> Items { get; }
    public ReadOnlyObservableCollection<HomeScreenItem> VisibleItems => _visibleItems;
    public ReadOnlyObservableCollection<HomeScreenItem> PinnedItems => _pinnedItems;
    public ReadOnlyObservableCollection<HomeScreenSection> VisibleSections => _visibleSections;
    public string SearchQuery { get => (string)GetValue(SearchQueryProperty); set => SetValue(SearchQueryProperty, value ?? string.Empty); }
    public bool IsEditMode { get => (bool)GetValue(IsEditModeProperty); set => SetValue(IsEditModeProperty, value); }
    public bool IsSearchEnabled { get => (bool)GetValue(IsSearchEnabledProperty); set => SetValue(IsSearchEnabledProperty, value); }
    public int MaxPinnedItems { get => (int)GetValue(MaxPinnedItemsProperty); set => SetValue(MaxPinnedItemsProperty, value); }
    public Func<CancellationToken, ValueTask<IReadOnlyList<HomeScreenItem>>>? RefreshProvider { get; set; }

    public event EventHandler<HomeScreenItemEventArgs>? ItemInvoked;
    public event EventHandler<HomeScreenPinChangedEventArgs>? PinChanged;
    public event EventHandler? LayoutChanged;
    public event EventHandler? RefreshStarted;
    public event EventHandler<Exception>? RefreshFailed;
    public event EventHandler? RefreshCompleted;

    public void ReplaceItems(IEnumerable<HomeScreenItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var snapshot = items.ToArray();
        ValidateUniqueIds(snapshot);
        Items.Clear();
        foreach (var item in snapshot) Items.Add(item);
        RecomputeViews();
    }

    public bool Invoke(string id)
    {
        var index = IndexOf(id);
        if (index < 0) return false;
        ItemInvoked?.Invoke(this, new HomeScreenItemEventArgs(Items[index], index));
        return true;
    }

    public bool TogglePin(string id) => IsPinned(id) ? Unpin(id) : Pin(id);

    public bool Pin(string id, int pinnedIndex = -1)
    {
        var sourceIndex = IndexOf(id);
        if (sourceIndex < 0 || Items[sourceIndex].IsPinned) return false;
        if (MaxPinnedItems > 0 && _pinnedBacking.Count >= MaxPinnedItems) return false;
        var current = Items[sourceIndex];
        var nextOrder = pinnedIndex < 0 ? NextPinnedOrder() : Math.Max(0, pinnedIndex);
        UpdateItem(sourceIndex, current with { IsPinned = true, Order = nextOrder });
        PinChanged?.Invoke(this, new HomeScreenPinChangedEventArgs(Items[sourceIndex], sourceIndex, true));
        LayoutChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool Unpin(string id)
    {
        var sourceIndex = IndexOf(id);
        if (sourceIndex < 0 || !Items[sourceIndex].IsPinned) return false;
        var current = Items[sourceIndex];
        UpdateItem(sourceIndex, current with { IsPinned = false, Order = 0 });
        NormalizePinnedOrder();
        PinChanged?.Invoke(this, new HomeScreenPinChangedEventArgs(Items[sourceIndex], sourceIndex, false));
        LayoutChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool MovePinned(string id, int destinationIndex)
    {
        var sourceIndex = IndexOf(id);
        if (sourceIndex < 0 || !Items[sourceIndex].IsPinned) return false;
        var pinned = _pinnedBacking.ToList();
        var current = pinned.FindIndex(item => item.Id == id);
        if (current < 0) return false;
        destinationIndex = Math.Clamp(destinationIndex, 0, pinned.Count - 1);
        if (current == destinationIndex) return true;
        var moved = pinned[current];
        pinned.RemoveAt(current);
        pinned.Insert(destinationIndex, moved);
        for (var i = 0; i < pinned.Count; i++)
        {
            var index = IndexOf(pinned[i].Id);
            UpdateItem(index, Items[index] with { Order = i });
        }
        RecomputeViews();
        LayoutChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public async ValueTask<bool> RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed || RefreshProvider is null) return false;
        CancelRefresh();
        var revision = Interlocked.Increment(ref _refreshRevision);
        _refreshCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = _refreshCts.Token;
        RefreshStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            var items = await RefreshProvider(token);
            token.ThrowIfCancellationRequested();
            if (_disposed || revision != _refreshRevision) return false;
            ReplaceItems(items);
            RefreshCompleted?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { return false; }
        catch (Exception ex)
        {
            if (!_disposed && revision == _refreshRevision) RefreshFailed?.Invoke(this, ex);
            return false;
        }
    }

    public IReadOnlyList<HomeScreenItem> CapturePinnedSnapshot() => _pinnedBacking.ToArray();

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        CancelRefresh();
        Items.CollectionChanged -= OnItemsChanged;
    }

    private bool IsPinned(string id) => IndexOf(id) is var index && index >= 0 && Items[index].IsPinned;
    private int IndexOf(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return -1;
        for (var index = 0; index < Items.Count; index++)
            if (string.Equals(Items[index].Id, id, StringComparison.Ordinal)) return index;
        return -1;
    }
    private int NextPinnedOrder() => _pinnedBacking.Count == 0 ? 0 : _pinnedBacking.Max(item => item.Order) + 1;

    private void UpdateItem(int index, HomeScreenItem item)
    {
        if (index < 0 || index >= Items.Count) return;
        Items[index] = item;
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) => RecomputeViews();
    private static void OnSearchQueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((HomeScreen)d).RecomputeViews();

    private void RecomputeViews()
    {
        if (_disposed) return;
        var query = IsSearchEnabled ? SearchQuery.Trim() : string.Empty;
        var filtered = Items.Where(item => query.Length == 0 || item.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase) || item.Subtitle?.Contains(query, StringComparison.CurrentCultureIgnoreCase) == true || item.Group?.Contains(query, StringComparison.CurrentCultureIgnoreCase) == true).ToArray();
        var pinned = filtered.Where(item => item.IsPinned).OrderBy(item => item.Order).ThenBy(item => Items.IndexOf(item)).ToArray();
        var unpinned = filtered.Where(item => !item.IsPinned).OrderBy(item => item.Group).ThenBy(item => item.Title, StringComparer.CurrentCultureIgnoreCase).ToArray();
        ReplaceCollection(_pinnedBacking, pinned);
        ReplaceCollection(_visibleBacking, pinned.Concat(unpinned));
        var sectionItems = filtered.GroupBy(item => string.IsNullOrWhiteSpace(item.Group) ? "default" : item.Group!, StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(group => group.Key == "default" ? 0 : 1)
            .ThenBy(group => group.Key, StringComparer.CurrentCultureIgnoreCase)
            .Select(group => new HomeScreenSection(group.Key, group.Key == "default" ? string.Empty : group.Key,
                group.OrderBy(item => item.IsPinned ? 0 : 1).ThenBy(item => item.Order).ThenBy(item => item.Title, StringComparer.CurrentCultureIgnoreCase).ToArray()))
            .ToArray();
        _sectionBacking.Clear();
        foreach (var section in sectionItems) _sectionBacking.Add(section);
    }

    private void NormalizePinnedOrder()
    {
        var pinned = Items.Where(item => item.IsPinned).OrderBy(item => item.Order).ThenBy(item => Items.IndexOf(item)).ToArray();
        for (var i = 0; i < pinned.Length; i++)
        {
            var index = IndexOf(pinned[i].Id);
            UpdateItem(index, Items[index] with { Order = i });
        }
        RecomputeViews();
    }

    private static void ReplaceCollection(ObservableCollection<HomeScreenItem> target, IEnumerable<HomeScreenItem> source)
    {
        target.Clear();
        foreach (var item in source) target.Add(item);
    }

    private void CancelRefresh()
    {
        Interlocked.Increment(ref _refreshRevision);
        _refreshCts?.Cancel();
        _refreshCts?.Dispose();
        _refreshCts = null;
    }

    private static void ValidateUniqueIds(IEnumerable<HomeScreenItem> items)
    {
        var values = items.ToArray();
        if (values.Select(static item => item.Id).Distinct(StringComparer.Ordinal).Count() != values.Length)
            throw new ArgumentException("Home screen item ids must be unique.", nameof(items));
    }
}
