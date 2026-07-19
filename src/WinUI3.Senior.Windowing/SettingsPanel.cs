using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace WinUI3.Senior.Windowing;

public sealed record SettingsPanelEntry(string Id, string Title, object? Content = null, string? ParentId = null);

public enum SettingsPanelNavigationKind
{
    Open,
    Forward,
    Back,
    Root,
    Close
}

public sealed class SettingsPanelNavigationEventArgs : EventArgs
{
    public SettingsPanelNavigationEventArgs(string? previousEntryId, string? currentEntryId, SettingsPanelNavigationKind kind)
        => (PreviousEntryId, CurrentEntryId, Kind) = (previousEntryId, currentEntryId, kind);

    public string? PreviousEntryId { get; }
    public string? CurrentEntryId { get; }
    public SettingsPanelNavigationKind Kind { get; }
}

/// <summary>Hierarchical settings flyout model with explicit navigation and Escape/back semantics.</summary>
public sealed class SettingsPanel : ContentControl
{
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen), typeof(bool), typeof(SettingsPanel), new PropertyMetadata(false, OnIsOpenChanged));

    public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register(
        nameof(IsModal), typeof(bool), typeof(SettingsPanel), new PropertyMetadata(true));

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(SettingsPanel), new PropertyMetadata("Settings"));

    private readonly Stack<string> _history = new();
    private readonly Dictionary<string, SettingsPanelEntry> _entries = new(StringComparer.Ordinal);
    private bool _opening;

    public SettingsPanel()
    {
        Entries = new ObservableCollection<SettingsPanelEntry>();
        Entries.CollectionChanged += (_, _) => RebuildIndex();
        KeyDown += OnKeyDown;
    }

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsModal
    {
        get => (bool)GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public ObservableCollection<SettingsPanelEntry> Entries { get; }
    public string? CurrentEntryId { get; private set; }
    public SettingsPanelEntry? CurrentEntry => CurrentEntryId is not null && _entries.TryGetValue(CurrentEntryId, out var entry) ? entry : null;
    public IReadOnlyList<SettingsPanelEntry> RootEntries => _entries.Values
        .Where(entry => string.IsNullOrWhiteSpace(entry.ParentId) || !_entries.ContainsKey(entry.ParentId!))
        .ToArray();
    public IReadOnlyList<SettingsPanelEntry> Breadcrumbs => BuildBreadcrumbs();
    public bool CanGoBack => _history.Count > 0;

    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public event EventHandler? NavigationChanged;
    public event EventHandler<SettingsPanelNavigationEventArgs>? NavigationChangedDetailed;

    public void Open(string? entryId = null)
    {
        if (_opening) return;
        _opening = true;
        try
        {
        var wasOpen = IsOpen;
        _history.Clear();
        if (!wasOpen)
        {
            CurrentEntryId = null;
            Content = null;
        }
        IsOpen = true;
        if (!string.IsNullOrWhiteSpace(entryId))
        {
            NavigateTo(entryId);
        }
        else if (CurrentEntryId is null)
        {
            var first = RootEntries.FirstOrDefault();
            if (first is not null)
            {
                NavigateToCore(first.Id, SettingsPanelNavigationKind.Open, addHistory: false);
            }
        }
        }
        finally { _opening = false; }
    }

    public void Close() => IsOpen = false;

    public bool NavigateTo(string entryId)
    {
        if (!_entries.TryGetValue(entryId, out var entry))
        {
            return false;
        }
        if (string.Equals(CurrentEntryId, entryId, StringComparison.Ordinal))
        {
            return true;
        }
        if (WouldCreateCycle(entry.Id))
        {
            return false;
        }
        if (CurrentEntryId is not null)
        {
            _history.Push(CurrentEntryId);
        }
        NavigateToCore(entryId, SettingsPanelNavigationKind.Forward, addHistory: false);
        return true;
    }

    public bool GoBack()
    {
        if (_history.Count == 0)
        {
            return false;
        }
        var previous = CurrentEntryId;
        string? target = null;
        while (_history.Count > 0 && target is null)
        {
            var candidate = _history.Pop();
            if (_entries.ContainsKey(candidate))
            {
                target = candidate;
            }
        }
        if (target is null)
        {
            return false;
        }
        NavigateToCore(target, SettingsPanelNavigationKind.Back, addHistory: false, previousEntryId: previous);
        return true;
    }

    public bool NavigateToRoot()
    {
        var root = RootEntries.FirstOrDefault();
        if (root is null)
        {
            return false;
        }
        var previous = CurrentEntryId;
        _history.Clear();
        NavigateToCore(root.Id, SettingsPanelNavigationKind.Root, addHistory: false, previousEntryId: previous);
        return true;
    }

    public bool HandleBack() => GoBack() || NavigateToRoot();

    public bool HandleEscape()
    {
        if (!IsOpen)
        {
            return false;
        }
        if (GoBack())
        {
            return true;
        }
        Close();
        return true;
    }

    private void RebuildIndex()
    {
        _entries.Clear();
        foreach (var entry in Entries.Where(static x => !string.IsNullOrWhiteSpace(x.Id)))
        {
            // Keep the first definition deterministic; a later duplicate must not
            // silently replace a page that may already be in the navigation stack.
            if (!_entries.ContainsKey(entry.Id)) _entries.Add(entry.Id, entry);
        }
        if (CurrentEntryId is not null && !_entries.ContainsKey(CurrentEntryId))
        {
            CurrentEntryId = null;
            Content = null;
            _history.Clear();
        }
    }

    private static void OnIsOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is SettingsPanel panel)
        {
            if ((bool)args.NewValue)
            {
                if (!panel._opening && panel.CurrentEntryId is null)
                {
                    var first = panel.RootEntries.FirstOrDefault();
                    if (first is not null) panel.NavigateToCore(first.Id, SettingsPanelNavigationKind.Open, addHistory: false);
                }
                panel.Opened?.Invoke(panel, EventArgs.Empty);
            }
            else
            {
                panel._history.Clear();
                var previous = panel.CurrentEntryId;
                panel.CurrentEntryId = null;
                panel.Content = null;
                panel.NavigationChanged?.Invoke(panel, EventArgs.Empty);
                panel.NavigationChangedDetailed?.Invoke(panel, new SettingsPanelNavigationEventArgs(previous, null, SettingsPanelNavigationKind.Close));
                panel.Closed?.Invoke(panel, EventArgs.Empty);
            }
        }
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (!IsOpen) return;
        if (e.Key == VirtualKey.Escape && HandleEscape()) e.Handled = true;
        else if (e.Key == VirtualKey.Left && e.KeyStatus.IsMenuDown && HandleBack()) e.Handled = true;
    }

    private void NavigateToCore(string entryId, SettingsPanelNavigationKind kind, bool addHistory, string? previousEntryId = null)
    {
        if (!_entries.TryGetValue(entryId, out var entry))
        {
            return;
        }
        var previous = previousEntryId ?? CurrentEntryId;
        if (addHistory && CurrentEntryId is not null)
        {
            _history.Push(CurrentEntryId);
        }
        CurrentEntryId = entryId;
        Content = entry.Content;
        NavigationChanged?.Invoke(this, EventArgs.Empty);
        NavigationChangedDetailed?.Invoke(this, new SettingsPanelNavigationEventArgs(previous, CurrentEntryId, kind));
    }

    private bool WouldCreateCycle(string entryId)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var current = entryId;
        while (_entries.TryGetValue(current, out var entry) && !string.IsNullOrWhiteSpace(entry.ParentId))
        {
            if (!visited.Add(current))
            {
                return true;
            }
            if (string.Equals(entry.ParentId, entryId, StringComparison.Ordinal))
            {
                return true;
            }
            current = entry.ParentId!;
        }
        return false;
    }

    private IReadOnlyList<SettingsPanelEntry> BuildBreadcrumbs()
    {
        if (CurrentEntryId is null || !_entries.TryGetValue(CurrentEntryId, out var current))
        {
            return Array.Empty<SettingsPanelEntry>();
        }
        var result = new List<SettingsPanelEntry>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        while (visited.Add(current.Id))
        {
            result.Insert(0, current);
            if (string.IsNullOrWhiteSpace(current.ParentId) || !_entries.TryGetValue(current.ParentId, out var parent))
            {
                break;
            }
            current = parent;
        }
        return result;
    }
}
