using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3.Senior.Controls;

public enum IconPickerState
{
    Loading,
    Ready,
    Empty
}

public sealed record IconPickerItem(string Id, string Name, string Glyph, string? Category = null, bool IsRtlMirrored = false, bool IsAvailable = true, object? Tag = null)
{
    public bool HasGlyph => IsAvailable && !string.IsNullOrEmpty(Glyph);
    public string CodePoint => HasGlyph ? string.Join(" ", Glyph.EnumerateRunes().Select(rune => $"U+{rune.Value:X4}")) : "Unavailable";
}

public sealed class IconPickerSource
{
    public IconPickerSource(string id, string displayName, IEnumerable<IconPickerItem>? items = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Source id is required.", nameof(id));
        Id = id.Trim();
        DisplayName = displayName ?? string.Empty;
        if (items is not null) SetItems(items);
        Items.CollectionChanged += (_, _) => ItemsChanged?.Invoke(this, EventArgs.Empty);
    }

    public string Id { get; }
    public string DisplayName { get; set; }
    public ObservableCollection<IconPickerItem> Items { get; } = new();
    public event EventHandler? ItemsChanged;

    public void SetItems(IEnumerable<IconPickerItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var unique = items.Where(item => item is not null && !string.IsNullOrWhiteSpace(item.Id))
            .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase).Select(group => group.First()).ToArray();
        Items.Clear();
        foreach (var item in unique) Items.Add(item);
    }
}

public sealed class IconPickerSelectionChangedEventArgs : EventArgs
{
    public IconPickerSelectionChangedEventArgs(IconPickerItem item, bool isUserInitiated) { Item = item; IsUserInitiated = isUserInitiated; }
    public IconPickerItem Item { get; }
    public bool IsUserInitiated { get; }
}

public sealed class IconPickerSelectionCommittedEventArgs : EventArgs
{
    public IconPickerSelectionCommittedEventArgs(IconPickerItem item) { Item = item; }
    public IconPickerItem Item { get; }
}

/// <summary>
/// An app-registered Fluent icon catalog. It never scans binaries or system
/// resources; hosts explicitly provide sources and own persistence.
/// </summary>
public sealed class IconPicker : Control
{
    private readonly ObservableCollection<IconPickerItem> _visibleIcons = new();
    private readonly HashSet<IconPickerSource> _subscribedSources = new();
    private readonly DispatcherQueueTimer _searchTimer;
    private TextBox? _searchBox;
    private bool _syncingSearch;
    private bool _selectionRequestedByUser;
    private bool _updatingCategories;
    private int _filterVersion;

    public static readonly DependencyProperty SelectedIconProperty = DependencyProperty.Register(
        nameof(SelectedIcon), typeof(IconPickerItem), typeof(IconPicker), new PropertyMetadata(null, OnSelectedIconChanged));
    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
        nameof(SearchText), typeof(string), typeof(IconPicker), new PropertyMetadata(string.Empty, OnSearchTextChanged));
    public static readonly DependencyProperty SelectedCategoryProperty = DependencyProperty.Register(
        nameof(SelectedCategory), typeof(string), typeof(IconPicker), new PropertyMetadata(null, OnFilterChanged));
    public static readonly DependencyProperty IsFavoritesOnlyProperty = DependencyProperty.Register(
        nameof(IsFavoritesOnly), typeof(bool), typeof(IconPicker), new PropertyMetadata(false, OnFilterChanged));
    public static readonly DependencyProperty SearchDebounceProperty = DependencyProperty.Register(
        nameof(SearchDebounce), typeof(TimeSpan), typeof(IconPicker), new PropertyMetadata(TimeSpan.FromMilliseconds(200), OnSearchDebounceChanged));
    public static readonly DependencyProperty PickerStateProperty = DependencyProperty.Register(
        nameof(PickerState), typeof(IconPickerState), typeof(IconPicker), new PropertyMetadata(IconPickerState.Empty, OnStateChanged));

    public IconPicker()
    {
        DefaultStyleKey = typeof(IconPicker);
        _searchTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _searchTimer.IsRepeating = false;
        _searchTimer.Tick += OnSearchTimerTick;
        Sources.CollectionChanged += OnSourcesChanged;
        Categories.CollectionChanged += (_, _) => RecomputeVisibleIcons();
        Favorites.CollectionChanged += (_, _) => RecomputeVisibleIcons();
        Recent.CollectionChanged += (_, _) => RecentChanged?.Invoke(this, EventArgs.Empty);
        IsTabStop = true;
        KeyDown += OnKeyDown;
        Unloaded += (_, _) => _searchTimer.Stop();
    }

    public ObservableCollection<IconPickerSource> Sources { get; } = new();
    public ObservableCollection<string> Categories { get; } = new();
    public ObservableCollection<string> Favorites { get; } = new();
    public ObservableCollection<string> Recent { get; } = new();
    public IReadOnlyList<IconPickerItem> VisibleIcons => _visibleIcons;
    public IconPickerItem? SelectedIcon { get => (IconPickerItem?)GetValue(SelectedIconProperty); set => SetValue(SelectedIconProperty, value); }
    public string SearchText { get => (string)GetValue(SearchTextProperty); set => SetValue(SearchTextProperty, value ?? string.Empty); }
    public string? SelectedCategory { get => (string?)GetValue(SelectedCategoryProperty); set => SetValue(SelectedCategoryProperty, value); }
    public bool IsFavoritesOnly { get => (bool)GetValue(IsFavoritesOnlyProperty); set => SetValue(IsFavoritesOnlyProperty, value); }
    public TimeSpan SearchDebounce { get => (TimeSpan)GetValue(SearchDebounceProperty); set => SetValue(SearchDebounceProperty, value < TimeSpan.Zero ? TimeSpan.Zero : value); }
    public IconPickerState PickerState { get => (IconPickerState)GetValue(PickerStateProperty); private set => SetValue(PickerStateProperty, value); }

    public event EventHandler? IconsChanged;
    public event EventHandler<IconPickerSelectionChangedEventArgs>? SelectionChanged;
    public event EventHandler<IconPickerSelectionCommittedEventArgs>? SelectionCommitted;
    public event EventHandler? RecentChanged;

    protected override AutomationPeer OnCreateAutomationPeer() => new IconPickerAutomationPeer(this);

    public void SetSources(IEnumerable<IconPickerSource> sources)
    {
        ArgumentNullException.ThrowIfNull(sources);
        Sources.Clear();
        foreach (var source in sources.Where(source => source is not null).GroupBy(source => source.Id, StringComparer.OrdinalIgnoreCase).Select(group => group.First())) Sources.Add(source);
        RecomputeVisibleIcons();
    }

    public void RegisterSource(IconPickerSource source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var existing = Sources.FirstOrDefault(item => string.Equals(item.Id, source.Id, StringComparison.OrdinalIgnoreCase));
        if (existing is not null) Sources.Remove(existing);
        Sources.Add(source);
    }

    public bool SelectIcon(string iconId, bool isUserInitiated = true)
    {
        if (string.IsNullOrWhiteSpace(iconId)) return false;
        var icon = Sources.SelectMany(source => source.Items).FirstOrDefault(item => string.Equals(item.Id, iconId, StringComparison.OrdinalIgnoreCase));
        if (icon is null || !icon.HasGlyph) return false;
        _selectionRequestedByUser = isUserInitiated;
        try { SelectedIcon = icon; }
        finally { _selectionRequestedByUser = false; }
        if (!Recent.Contains(icon.Id, StringComparer.OrdinalIgnoreCase)) Recent.Insert(0, icon.Id);
        else
        {
            Recent.Remove(Recent.First(item => string.Equals(item, icon.Id, StringComparison.OrdinalIgnoreCase)));
            Recent.Insert(0, icon.Id);
        }
        while (Recent.Count > 24) Recent.RemoveAt(Recent.Count - 1);
        return true;
    }

    public bool CommitSelection()
    {
        if (SelectedIcon is null || !SelectedIcon.HasGlyph) return false;
        SelectionCommitted?.Invoke(this, new IconPickerSelectionCommittedEventArgs(SelectedIcon));
        return true;
    }

    public bool ToggleFavorite(string iconId)
    {
        var icon = Sources.SelectMany(source => source.Items).FirstOrDefault(item => string.Equals(item.Id, iconId, StringComparison.OrdinalIgnoreCase));
        if (icon is null) return false;
        var existing = Favorites.FirstOrDefault(item => string.Equals(item, icon.Id, StringComparison.OrdinalIgnoreCase));
        if (existing is null) Favorites.Add(icon.Id); else Favorites.Remove(existing);
        return true;
    }

    public bool IsFavorite(string iconId) => Favorites.Any(item => string.Equals(item, iconId, StringComparison.OrdinalIgnoreCase));

    public IconPickerItem? ResolveRecent(string iconId) => Sources.SelectMany(source => source.Items).FirstOrDefault(item => string.Equals(item.Id, iconId, StringComparison.OrdinalIgnoreCase));

    public void RecomputeVisibleIcons()
    {
        var version = ++_filterVersion;
        _visibleIcons.Clear();
        var query = SearchText.Trim();
        var category = SelectedCategory?.Trim();
        var discoveredCategories = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        foreach (var item in Sources.SelectMany(source => source.Items))
        {
            if (!item.IsAvailable || !item.HasGlyph) continue;
            if (IsFavoritesOnly && !IsFavorite(item.Id)) continue;
            if (!string.IsNullOrWhiteSpace(category) && !string.Equals(category, item.Category, StringComparison.CurrentCultureIgnoreCase)) continue;
            if (query.Length > 0 && !item.Id.Contains(query, StringComparison.CurrentCultureIgnoreCase) && !item.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) && !string.Equals(item.Glyph, query, StringComparison.Ordinal)) continue;
            _visibleIcons.Add(item);
            if (!string.IsNullOrWhiteSpace(item.Category)) discoveredCategories.Add(item.Category);
        }
        // Do not mutate Categories while filtering: its CollectionChanged handler
        // recomputes the view and would otherwise recurse for every new category.
        if (!_updatingCategories)
        {
            _updatingCategories = true;
            try
            {
                foreach (var value in discoveredCategories.OrderBy(value => value, StringComparer.CurrentCultureIgnoreCase))
                    if (!Categories.Contains(value, StringComparer.CurrentCultureIgnoreCase)) Categories.Add(value);
            }
            finally { _updatingCategories = false; }
        }
        PickerState = _visibleIcons.Count == 0 ? IconPickerState.Empty : IconPickerState.Ready;
        IconsChanged?.Invoke(this, EventArgs.Empty);
        if (version != _filterVersion) return;
        UpdateVisualState();
    }

    protected override void OnApplyTemplate()
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate();
        _searchBox = GetTemplateChild("PART_SearchBox") as TextBox;
        if (_searchBox is not null)
        {
            _searchBox.Text = SearchText;
            _searchBox.TextChanged += OnSearchBoxTextChanged;
            _searchBox.KeyDown += OnSearchBoxKeyDown;
        }
        RecomputeVisibleIcons();
    }

    private void DetachTemplateHandlers()
    {
        if (_searchBox is null) return;
        _searchBox.TextChanged -= OnSearchBoxTextChanged;
        _searchBox.KeyDown -= OnSearchBoxKeyDown;
    }

    private static void OnSelectedIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (IconPicker)d;
        owner.UpdateVisualState();
        if (e.NewValue is IconPickerItem item && !ReferenceEquals(e.OldValue, item)) owner.SelectionChanged?.Invoke(owner, new IconPickerSelectionChangedEventArgs(item, owner._selectionRequestedByUser));
    }
    private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (IconPicker)d;
        if (!owner._syncingSearch && owner._searchBox is not null)
        {
            owner._syncingSearch = true;
            try { owner._searchBox.Text = (string)e.NewValue; } finally { owner._syncingSearch = false; }
        }
        owner._searchTimer.Stop();
        if (owner.SearchDebounce == TimeSpan.Zero) owner.RecomputeVisibleIcons();
        else { owner._searchTimer.Interval = owner.SearchDebounce; owner._searchTimer.Start(); }
    }
    private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((IconPicker)d).RecomputeVisibleIcons();
    private static void OnSearchDebounceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((IconPicker)d)._searchTimer.Interval = ((IconPicker)d).SearchDebounce;
    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((IconPicker)d).UpdateVisualState();
    private void OnSourcesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (IconPickerSource source in e.OldItems)
            {
                if (_subscribedSources.Remove(source)) source.ItemsChanged -= OnSourceItemsChanged;
            }
        }
        if (e.NewItems is not null)
        {
            foreach (IconPickerSource source in e.NewItems)
            {
                if (_subscribedSources.Add(source)) source.ItemsChanged += OnSourceItemsChanged;
            }
        }
        RecomputeVisibleIcons();
    }
    private void OnSourceItemsChanged(object? sender, EventArgs e) => RecomputeVisibleIcons();
    private void OnSearchBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_syncingSearch) return;
        _syncingSearch = true;
        try { SearchText = _searchBox?.Text ?? string.Empty; } finally { _syncingSearch = false; }
    }
    private void OnSearchBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter) { CommitSelection(); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.Escape) { SearchText = string.Empty; e.Handled = true; }
    }
    private void OnSearchTimerTick(DispatcherQueueTimer sender, object args) => RecomputeVisibleIcons();
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter && SelectedIcon is not null) { CommitSelection(); e.Handled = true; }
    }
    private void UpdateVisualState() => VisualStateManager.GoToState(this, PickerState.ToString(), true);
}

internal sealed class IconPickerAutomationPeer(IconPicker owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.List;
    protected override string GetClassNameCore() => nameof(IconPicker);
    protected override string GetNameCore() => AutomationProperties.GetName(Owner) is { Length: > 0 } name ? name : "Icon picker";
    protected override bool IsKeyboardFocusableCore() => Owner is IconPicker picker && picker.IsEnabled && picker.IsTabStop;
}
