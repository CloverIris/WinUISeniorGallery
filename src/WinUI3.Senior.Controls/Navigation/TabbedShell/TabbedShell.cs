using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;

namespace WinUI3.Senior.Controls;

public sealed class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        value is bool isVisible && isVisible ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        value is Visibility visibility && visibility == Visibility.Visible;
}

public enum TabbedShellCloseReason
{
    User,
    Host,
    Replacement,
}

public sealed class TabbedShellItem : INotifyPropertyChanged
{
    private string _header;
    private object? _content;
    private string? _iconGlyph;
    private bool _canClose = true;
    private bool _isPinned;

    public TabbedShellItem(string id, string header, object? content = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Tab id is required.", nameof(id));
        Id = id.Trim();
        _header = header ?? string.Empty;
        _content = content;
    }

    public string Id { get; }
    public string Header { get => _header; set => Set(ref _header, value ?? string.Empty); }
    public object? Content { get => _content; set => Set(ref _content, value); }
    public string? IconGlyph { get => _iconGlyph; set => Set(ref _iconGlyph, value); }
    public bool CanClose { get => _canClose; set => Set(ref _canClose, value); }
    public bool IsPinned { get => _isPinned; set => Set(ref _isPinned, value); }
    public object? Tag { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed class TabbedShellTabClosingEventArgs(TabbedShellItem item, TabbedShellCloseReason reason, bool isUserInitiated) : EventArgs
{
    public TabbedShellItem Item { get; } = item ?? throw new ArgumentNullException(nameof(item));
    public TabbedShellCloseReason Reason { get; } = reason;
    public bool IsUserInitiated { get; } = isUserInitiated;
    public bool Cancel { get; set; }
}

public sealed class TabbedShellTabClosedEventArgs(TabbedShellItem item, TabbedShellCloseReason reason, bool isUserInitiated) : EventArgs
{
    public TabbedShellItem Item { get; } = item ?? throw new ArgumentNullException(nameof(item));
    public TabbedShellCloseReason Reason { get; } = reason;
    public bool IsUserInitiated { get; } = isUserInitiated;
}

public sealed class TabbedShellTabReorderedEventArgs(TabbedShellItem item, int oldIndex, int newIndex) : EventArgs
{
    public TabbedShellItem Item { get; } = item ?? throw new ArgumentNullException(nameof(item));
    public int OldIndex { get; } = oldIndex;
    public int NewIndex { get; } = newIndex;
}

public sealed class TabbedShellTearOutRequestedEventArgs(TabbedShellItem item) : EventArgs
{
    public TabbedShellItem Item { get; } = item ?? throw new ArgumentNullException(nameof(item));
    public bool Handled { get; set; }
}

/// <summary>
/// A host-neutral tab container with keyboard navigation, close guards, reorder operations
/// and an explicit tear-out request. It never creates a new Window or moves content itself.
/// </summary>
[TemplatePart(Name = "PART_TabList", Type = typeof(ListView))]
[TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
public sealed class TabbedShell : ContentControl
{
    private readonly ObservableCollection<TabbedShellItem> _items = new();
    private ListView? _tabList;
    private bool _syncingSelection;
    private bool _mutatingItems;

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem), typeof(TabbedShellItem), typeof(TabbedShell), new PropertyMetadata(null, OnSelectedItemChanged));
    public static readonly DependencyProperty IsCloseButtonsVisibleProperty = DependencyProperty.Register(
        nameof(IsCloseButtonsVisible), typeof(bool), typeof(TabbedShell), new PropertyMetadata(true, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsReorderEnabledProperty = DependencyProperty.Register(
        nameof(IsReorderEnabled), typeof(bool), typeof(TabbedShell), new PropertyMetadata(true));
    public static readonly DependencyProperty IsTearOutEnabledProperty = DependencyProperty.Register(
        nameof(IsTearOutEnabled), typeof(bool), typeof(TabbedShell), new PropertyMetadata(true));
    public static readonly DependencyProperty IsKeyboardNavigationEnabledProperty = DependencyProperty.Register(
        nameof(IsKeyboardNavigationEnabled), typeof(bool), typeof(TabbedShell), new PropertyMetadata(true));

    public TabbedShell()
    {
        DefaultStyleKey = typeof(TabbedShell);
        _items.CollectionChanged += OnItemsChanged;
        IsTabStop = true;
        KeyDown += OnKeyDown;
    }

    public ObservableCollection<TabbedShellItem> Items => _items;
    public TabbedShellItem? SelectedItem { get => (TabbedShellItem?)GetValue(SelectedItemProperty); private set => SetValue(SelectedItemProperty, value); }
    public int SelectedIndex => SelectedItem is null ? -1 : _items.IndexOf(SelectedItem);
    public bool IsCloseButtonsVisible { get => (bool)GetValue(IsCloseButtonsVisibleProperty); set => SetValue(IsCloseButtonsVisibleProperty, value); }
    public bool IsReorderEnabled { get => (bool)GetValue(IsReorderEnabledProperty); set => SetValue(IsReorderEnabledProperty, value); }
    public bool IsTearOutEnabled { get => (bool)GetValue(IsTearOutEnabledProperty); set => SetValue(IsTearOutEnabledProperty, value); }
    public bool IsKeyboardNavigationEnabled { get => (bool)GetValue(IsKeyboardNavigationEnabledProperty); set => SetValue(IsKeyboardNavigationEnabledProperty, value); }

    public event EventHandler? SelectionChanged;
    public event EventHandler<TabbedShellTabClosingEventArgs>? TabClosing;
    public event EventHandler<TabbedShellTabClosedEventArgs>? TabClosed;
    public event EventHandler<TabbedShellTabReorderedEventArgs>? TabReordered;
    public event EventHandler<TabbedShellTearOutRequestedEventArgs>? TearOutRequested;

    protected override AutomationPeer OnCreateAutomationPeer() => new TabbedShellAutomationPeer(this);

    public void SelectTab(string id)
    {
        var item = _items.FirstOrDefault(candidate => string.Equals(candidate.Id, id, StringComparison.Ordinal));
        if (item is not null) SelectTab(item);
    }

    public void SelectTab(TabbedShellItem? item)
    {
        if (item is not null && !_items.Contains(item)) return;
        SelectedItem = item;
    }

    public TabbedShellItem AddTab(TabbedShellItem item, bool select = true, int? index = null)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_items.Any(existing => string.Equals(existing.Id, item.Id, StringComparison.Ordinal)))
            throw new ArgumentException($"A tab with id '{item.Id}' already exists.", nameof(item));
        _mutatingItems = true;
        try
        {
            if (index is { } requestedIndex)
                _items.Insert(Math.Clamp(requestedIndex, 0, _items.Count), item);
            else
                _items.Add(item);
        }
        finally { _mutatingItems = false; }
        if (select) SelectTab(item);
        return item;
    }

    public bool CloseTab(string id, TabbedShellCloseReason reason = TabbedShellCloseReason.User, bool isUserInitiated = true) =>
        CloseTab(_items.FirstOrDefault(item => string.Equals(item.Id, id, StringComparison.Ordinal)), reason, isUserInitiated);

    public bool CloseTab(TabbedShellItem? item, TabbedShellCloseReason reason = TabbedShellCloseReason.User, bool isUserInitiated = true)
    {
        if (item is null || !_items.Contains(item) || !item.CanClose) return false;
        var args = new TabbedShellTabClosingEventArgs(item, reason, isUserInitiated);
        TabClosing?.Invoke(this, args);
        if (args.Cancel) return false;
        var oldIndex = _items.IndexOf(item);
        var wasSelected = ReferenceEquals(item, SelectedItem);
        _mutatingItems = true;
        try { _items.RemoveAt(oldIndex); }
        finally { _mutatingItems = false; }
        if (wasSelected)
        {
            var replacementIndex = Math.Min(oldIndex, _items.Count - 1);
            SelectTab(replacementIndex >= 0 ? _items[replacementIndex] : null);
        }
        TabClosed?.Invoke(this, new TabbedShellTabClosedEventArgs(item, reason, isUserInitiated));
        return true;
    }

    public bool MoveTab(string id, int newIndex) => MoveTab(_items.FirstOrDefault(item => string.Equals(item.Id, id, StringComparison.Ordinal)), newIndex);

    public bool MoveTab(TabbedShellItem? item, int newIndex)
    {
        if (!IsReorderEnabled || item is null) return false;
        var oldIndex = _items.IndexOf(item);
        if (oldIndex < 0) return false;
        var target = Math.Clamp(newIndex, 0, _items.Count - 1);
        if (oldIndex == target) return false;
        _items.Move(oldIndex, target);
        TabReordered?.Invoke(this, new TabbedShellTabReorderedEventArgs(item, oldIndex, target));
        return true;
    }

    public bool RequestTearOut(TabbedShellItem? item = null)
    {
        if (!IsTearOutEnabled) return false;
        var target = item ?? SelectedItem;
        if (target is null || !_items.Contains(target)) return false;
        var args = new TabbedShellTearOutRequestedEventArgs(target);
        TearOutRequested?.Invoke(this, args);
        return args.Handled;
    }

    public bool SelectNext(bool reverse = false)
    {
        if (_items.Count == 0) return false;
        var current = SelectedIndex;
        var next = current < 0 ? 0 : (current + (reverse ? -1 : 1) + _items.Count) % _items.Count;
        SelectTab(_items[next]);
        return true;
    }

    protected override void OnApplyTemplate()
    {
        if (_tabList is not null)
        {
            _tabList.SelectionChanged -= OnTabListSelectionChanged;
            _tabList.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnTabCloseClick));
        }
        base.OnApplyTemplate();
        _tabList = GetTemplateChild("PART_TabList") as ListView;
        if (_tabList is not null)
        {
            _tabList.SelectionChanged += OnTabListSelectionChanged;
            _tabList.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnTabCloseClick), true);
            _tabList.SelectedItem = SelectedItem;
        }
        UpdateVisualState();
    }

    private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var shell = (TabbedShell)sender;
        if (!shell._syncingSelection && shell._tabList is not null)
            shell._tabList.SelectedItem = args.NewValue;
        shell.SelectionChanged?.Invoke(shell, EventArgs.Empty);
        shell.UpdateVisualState();
    }

    private static void OnVisualPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) => ((TabbedShell)sender).UpdateVisualState();

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        var duplicate = _items.GroupBy(item => item.Id, StringComparer.Ordinal).FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null) throw new InvalidOperationException($"Duplicate tab id '{duplicate.Key}'.");
        if (!_mutatingItems)
        {
            if (SelectedItem is null && _items.Count > 0) SelectTab(_items[0]);
            else if (SelectedItem is not null && !_items.Contains(SelectedItem)) SelectTab(_items.FirstOrDefault());
        }
        UpdateVisualState();
    }

    private void OnTabListSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        if (_syncingSelection) return;
        _syncingSelection = true;
        try { SelectedItem = _tabList?.SelectedItem as TabbedShellItem; }
        finally { _syncingSelection = false; }
    }

    private void OnTabCloseClick(object sender, RoutedEventArgs args)
    {
        if (args.OriginalSource is FrameworkElement element && element.DataContext is TabbedShellItem item)
        {
            if (CloseTab(item)) args.Handled = true;
        }
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs args)
    {
        if (!IsKeyboardNavigationEnabled || !args.KeyStatus.IsKeyDown) return;
        var ctrl = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
        var shift = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);
        if (ctrl && args.Key == VirtualKey.Tab) { SelectNext(shift); args.Handled = true; return; }
        if (ctrl && args.Key == VirtualKey.W) { if (CloseTab(SelectedItem)) args.Handled = true; return; }
        if (ctrl && args.Key == VirtualKey.T) { if (RequestTearOut()) args.Handled = true; return; }
        if (ctrl && args.Key >= VirtualKey.Number1 && args.Key <= VirtualKey.Number9)
        {
            var index = (int)args.Key - (int)VirtualKey.Number1;
            if (index < _items.Count) { SelectTab(_items[index]); args.Handled = true; }
        }
    }

    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, SelectedItem is null ? "Empty" : "HasSelection", true);
        if (_tabList is not null) _tabList.SelectedItem = SelectedItem;
    }
}

internal sealed class TabbedShellAutomationPeer(TabbedShell owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Tab;
    protected override string GetClassNameCore() => nameof(TabbedShell);
    protected override string GetNameCore() => AutomationProperties.GetName(Owner) is { Length: > 0 } name ? name : "Tabbed shell";
}
