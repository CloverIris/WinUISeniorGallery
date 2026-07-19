using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3.Senior.Controls;

public enum CommandRibbonCollapseMode
{
    Auto,
    AlwaysExpanded,
    AlwaysMinimized
}

public enum CommandRibbonCommandDisplayMode
{
    Large,
    Medium,
    Small,
    Overflow
}

public sealed class CommandRibbonCommand : INotifyPropertyChanged
{
    private string _label;
    private string? _keyTip;
    private string? _iconGlyph;
    private bool _isEnabled = true;
    private bool _isChecked;
    private bool _isToggle;
    private int _priority;
    private CommandRibbonCommandDisplayMode _preferredDisplayMode = CommandRibbonCommandDisplayMode.Large;

    public CommandRibbonCommand(string id, string label, ICommand? command = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Command id is required.", nameof(id));
        Id = id.Trim();
        _label = label ?? string.Empty;
        Command = command;
    }

    public string Id { get; }
    public string Label { get => _label; set => Set(ref _label, value ?? string.Empty); }
    public string? KeyTip { get => _keyTip; set => Set(ref _keyTip, NormalizeKeyTip(value)); }
    public string? IconGlyph { get => _iconGlyph; set => Set(ref _iconGlyph, value); }
    public bool IsEnabled { get => _isEnabled; set => Set(ref _isEnabled, value); }
    public bool IsChecked { get => _isChecked; set => Set(ref _isChecked, value); }
    public bool IsToggle { get => _isToggle; set => Set(ref _isToggle, value); }
    public int Priority { get => _priority; set => Set(ref _priority, value); }
    public CommandRibbonCommandDisplayMode PreferredDisplayMode { get => _preferredDisplayMode; set => Set(ref _preferredDisplayMode, value); }
    public ICommand? Command { get; set; }
    public object? Tag { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? Invoked;

    public bool TryInvoke()
    {
        if (!IsEnabled || Command?.CanExecute(Tag) == false) return false;
        if (IsToggle) IsChecked = !IsChecked;
        Command?.Execute(Tag);
        Invoked?.Invoke(this, EventArgs.Empty);
        return true;
    }

    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static string? NormalizeKeyTip(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Trim().ToUpperInvariant();
    }
}

public sealed class CommandRibbonGroup : INotifyPropertyChanged
{
    private string _label;
    private bool _isVisible = true;
    private int _priority;

    public CommandRibbonGroup(string id, string label)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Group id is required.", nameof(id));
        Id = id.Trim();
        _label = label ?? string.Empty;
        Commands.CollectionChanged += OnCommandsChanged;
    }

    public string Id { get; }
    public string Label { get => _label; set => Set(ref _label, value ?? string.Empty); }
    public bool IsVisible { get => _isVisible; set => Set(ref _isVisible, value); }
    public int Priority { get => _priority; set => Set(ref _priority, value); }
    public ObservableCollection<CommandRibbonCommand> Commands { get; } = new();
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? CommandsChanged;

    private void OnCommandsChanged(object? sender, NotifyCollectionChangedEventArgs e) => CommandsChanged?.Invoke(this, EventArgs.Empty);
    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed class CommandRibbonTab : INotifyPropertyChanged
{
    private string _label;
    private bool _isVisible = true;
    private bool _isContextual;

    public CommandRibbonTab(string id, string label)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Tab id is required.", nameof(id));
        Id = id.Trim();
        _label = label ?? string.Empty;
        Groups.CollectionChanged += OnGroupsChanged;
    }

    public string Id { get; }
    public string Label { get => _label; set => Set(ref _label, value ?? string.Empty); }
    public bool IsVisible { get => _isVisible; set => Set(ref _isVisible, value); }
    public bool IsContextual { get => _isContextual; set => Set(ref _isContextual, value); }
    public ObservableCollection<CommandRibbonGroup> Groups { get; } = new();
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? GroupsChanged;

    private void OnGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e) => GroupsChanged?.Invoke(this, EventArgs.Empty);
    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed class CommandRibbonCommandInvokedEventArgs : EventArgs
{
    public CommandRibbonCommandInvokedEventArgs(CommandRibbonCommand command, bool fromOverflow, bool isUserInitiated)
    {
        Command = command;
        FromOverflow = fromOverflow;
        IsUserInitiated = isUserInitiated;
    }

    public CommandRibbonCommand Command { get; }
    public bool FromOverflow { get; }
    public bool IsUserInitiated { get; }
}

public sealed class CommandRibbonLayoutChangedEventArgs : EventArgs
{
    public CommandRibbonLayoutChangedEventArgs(double availableWidth, IReadOnlyList<CommandRibbonCommand> overflowCommands)
    {
        AvailableWidth = availableWidth;
        OverflowCommands = overflowCommands;
    }

    public double AvailableWidth { get; }
    public IReadOnlyList<CommandRibbonCommand> OverflowCommands { get; }
}

/// <summary>
/// A host-fed contextual command ribbon. It owns grouping, key tips, responsive
/// command density and overflow ordering; the host owns command side effects.
/// </summary>
public sealed class CommandRibbon : Control
{
    private readonly ObservableCollection<CommandRibbonCommand> _visibleCommands = new();
    private readonly ObservableCollection<CommandRibbonCommand> _overflowCommands = new();
    private readonly Dictionary<string, CommandRibbonCommand> _commandsById = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<CommandRibbonGroup> _subscribedGroups = new();
    private readonly HashSet<CommandRibbonCommand> _subscribedCommands = new();
    private bool _isApplyingSelection;
    private double _lastAvailableWidth = double.PositiveInfinity;

    public static readonly DependencyProperty SelectedTabProperty = DependencyProperty.Register(
        nameof(SelectedTab), typeof(CommandRibbonTab), typeof(CommandRibbon), new PropertyMetadata(null, OnSelectionChanged));
    public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register(
        nameof(IsMinimized), typeof(bool), typeof(CommandRibbon), new PropertyMetadata(false, OnMinimizedChanged));
    public static readonly DependencyProperty CollapseModeProperty = DependencyProperty.Register(
        nameof(CollapseMode), typeof(CommandRibbonCollapseMode), typeof(CommandRibbon), new PropertyMetadata(CommandRibbonCollapseMode.Auto, OnLayoutPropertyChanged));
    public static readonly DependencyProperty CollapseThresholdProperty = DependencyProperty.Register(
        nameof(CollapseThreshold), typeof(double), typeof(CommandRibbon), new PropertyMetadata(680d, OnLayoutPropertyChanged));
    public static readonly DependencyProperty IsKeyTipModeEnabledProperty = DependencyProperty.Register(
        nameof(IsKeyTipModeEnabled), typeof(bool), typeof(CommandRibbon), new PropertyMetadata(false));
    public static readonly DependencyProperty IsContextualGroupsEnabledProperty = DependencyProperty.Register(
        nameof(IsContextualGroupsEnabled), typeof(bool), typeof(CommandRibbon), new PropertyMetadata(true, OnLayoutPropertyChanged));

    public CommandRibbon()
    {
        DefaultStyleKey = typeof(CommandRibbon);
        Tabs.CollectionChanged += OnTabsChanged;
        ContextualGroups.CollectionChanged += OnContextualGroupsChanged;
        SizeChanged += OnSizeChanged;
        KeyDown += OnKeyDown;
        IsTabStop = true;
    }

    public ObservableCollection<CommandRibbonTab> Tabs { get; } = new();
    public ObservableCollection<CommandRibbonGroup> ContextualGroups { get; } = new();
    public CommandRibbonTab? SelectedTab { get => (CommandRibbonTab?)GetValue(SelectedTabProperty); set => SetValue(SelectedTabProperty, value); }
    public bool IsMinimized { get => (bool)GetValue(IsMinimizedProperty); set => SetValue(IsMinimizedProperty, value); }
    public CommandRibbonCollapseMode CollapseMode { get => (CommandRibbonCollapseMode)GetValue(CollapseModeProperty); set => SetValue(CollapseModeProperty, value); }
    public double CollapseThreshold { get => (double)GetValue(CollapseThresholdProperty); set => SetValue(CollapseThresholdProperty, Math.Max(0, value)); }
    public bool IsKeyTipModeEnabled { get => (bool)GetValue(IsKeyTipModeEnabledProperty); set => SetValue(IsKeyTipModeEnabledProperty, value); }
    public bool IsContextualGroupsEnabled { get => (bool)GetValue(IsContextualGroupsEnabledProperty); set => SetValue(IsContextualGroupsEnabledProperty, value); }
    public IReadOnlyList<CommandRibbonCommand> VisibleCommands => _visibleCommands;
    public IReadOnlyList<CommandRibbonCommand> OverflowCommands => _overflowCommands;

    public event EventHandler? SelectionChanged;
    public event EventHandler<CommandRibbonCommandInvokedEventArgs>? CommandInvoked;
    public event EventHandler<CommandRibbonLayoutChangedEventArgs>? LayoutChanged;
    public event EventHandler? OverflowRequested;

    protected override AutomationPeer OnCreateAutomationPeer() => new CommandRibbonAutomationPeer(this);

    public void SelectTab(string tabId)
    {
        var tab = Tabs.FirstOrDefault(item => string.Equals(item.Id, tabId, StringComparison.OrdinalIgnoreCase));
        if (tab is null || !tab.IsVisible) return;
        SelectedTab = tab;
    }

    public void ToggleMinimized() => IsMinimized = !IsMinimized;

    /// <summary>
    /// Rebuilds the active command index and responsive layout after a host
    /// mutates a command, group, or command implementation in place.
    /// </summary>
    public void RefreshCommands() => RecomputeLayout(_lastAvailableWidth);

    public void RecomputeLayout(double availableWidth)
    {
        _lastAvailableWidth = double.IsNaN(availableWidth) || availableWidth < 0 ? 0 : availableWidth;
        if (CollapseMode == CommandRibbonCollapseMode.AlwaysExpanded) IsMinimized = false;
        else if (CollapseMode == CommandRibbonCollapseMode.AlwaysMinimized) IsMinimized = true;
        else if (_lastAvailableWidth > 0) IsMinimized = _lastAvailableWidth < CollapseThreshold;

        _visibleCommands.Clear();
        _overflowCommands.Clear();
        RebuildCommandIndex();
        var candidates = ActiveGroups().SelectMany(group => group.Commands.Where(command => command.IsEnabled)).ToArray();
        if (IsMinimized)
        {
            foreach (var command in candidates) _overflowCommands.Add(command);
        }
        else
        {
            var budget = Math.Max(0, _lastAvailableWidth - 120);
            foreach (var command in candidates.OrderByDescending(item => item.Priority).ThenBy(item => item.Id, StringComparer.OrdinalIgnoreCase))
            {
                var preferred = WidthFor(command.PreferredDisplayMode);
                if (budget >= preferred)
                {
                    budget -= preferred;
                    _visibleCommands.Add(command);
                }
                else if (budget >= WidthFor(CommandRibbonCommandDisplayMode.Small))
                {
                    budget -= WidthFor(CommandRibbonCommandDisplayMode.Small);
                    _visibleCommands.Add(command);
                }
                else _overflowCommands.Add(command);
            }
        }
        LayoutChanged?.Invoke(this, new CommandRibbonLayoutChangedEventArgs(_lastAvailableWidth, _overflowCommands.ToArray()));
        UpdateVisualState();
    }

    public bool InvokeCommand(string commandId, bool fromOverflow = false, bool isUserInitiated = true)
    {
        if (string.IsNullOrWhiteSpace(commandId) || !_commandsById.TryGetValue(commandId, out var command)) return false;
        if (!IsCommandActive(command)) return false;
        if (!command.TryInvoke()) return false;
        CommandInvoked?.Invoke(this, new CommandRibbonCommandInvokedEventArgs(command, fromOverflow || _overflowCommands.Contains(command), isUserInitiated));
        return true;
    }

    public bool InvokeKeyTip(string keyTip)
    {
        var normalized = keyTip?.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(normalized)) return false;
        var matches = ActiveGroups().SelectMany(group => group.Commands).Where(item => string.Equals(item.KeyTip, normalized, StringComparison.OrdinalIgnoreCase)).ToArray();
        return matches.Length == 1 && InvokeCommand(matches[0].Id, _overflowCommands.Contains(matches[0]));
    }

    public void RequestOverflow() => OverflowRequested?.Invoke(this, EventArgs.Empty);

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateVisualState();
        RecomputeLayout(ActualWidth > 0 ? ActualWidth : _lastAvailableWidth);
    }

    private IEnumerable<CommandRibbonGroup> ActiveGroups()
    {
        if (SelectedTab is { IsVisible: true })
        {
            foreach (var group in SelectedTab.Groups.Where(item => item.IsVisible)) yield return group;
        }
        if (IsContextualGroupsEnabled)
        {
            foreach (var group in ContextualGroups.Where(item => item.IsVisible)) yield return group;
        }
    }

    private bool IsCommandActive(CommandRibbonCommand command) => ActiveGroups().Any(group => group.Commands.Contains(command));
    private void RebuildCommandIndex()
    {
        foreach (var group in _subscribedGroups) group.CommandsChanged -= OnGroupCommandsChanged;
        foreach (var command in _subscribedCommands) command.PropertyChanged -= OnCommandPropertyChanged;
        _subscribedGroups.Clear();
        _subscribedCommands.Clear();
        _commandsById.Clear();
        foreach (var group in ActiveGroups())
        {
            if (_subscribedGroups.Add(group)) group.CommandsChanged += OnGroupCommandsChanged;
            foreach (var command in group.Commands)
            {
                if (_subscribedCommands.Add(command)) command.PropertyChanged += OnCommandPropertyChanged;
                if (!_commandsById.ContainsKey(command.Id)) _commandsById.Add(command.Id, command);
            }
        }
    }
    private static double WidthFor(CommandRibbonCommandDisplayMode mode) => mode switch { CommandRibbonCommandDisplayMode.Large => 88, CommandRibbonCommandDisplayMode.Medium => 64, _ => 44 };
    private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (CommandRibbon)d;
        owner.RecomputeLayout(owner._lastAvailableWidth);
        owner.SelectionChanged?.Invoke(owner, EventArgs.Empty);
    }
    private static void OnMinimizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CommandRibbon)d).RecomputeLayout(((CommandRibbon)d)._lastAvailableWidth);
    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CommandRibbon)d).RecomputeLayout(((CommandRibbon)d)._lastAvailableWidth);
    private void OnTabsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (SelectedTab is null || !Tabs.Contains(SelectedTab)) SelectedTab = Tabs.FirstOrDefault(item => item.IsVisible);
        RecomputeLayout(_lastAvailableWidth);
    }
    private void OnContextualGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e) => RecomputeLayout(_lastAvailableWidth);
    private void OnGroupCommandsChanged(object? sender, EventArgs e) => RecomputeLayout(_lastAvailableWidth);
    private void OnCommandPropertyChanged(object? sender, PropertyChangedEventArgs e) => RecomputeLayout(_lastAvailableWidth);
    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => RecomputeLayout(e.NewSize.Width);
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.F10)
        {
            ToggleMinimized();
            e.Handled = true;
            return;
        }
        if (e.Key == Windows.System.VirtualKey.F6)
        {
            Focus(FocusState.Keyboard);
            e.Handled = true;
            return;
        }
        if (e.Key == Windows.System.VirtualKey.Escape && IsKeyTipModeEnabled)
        {
            IsKeyTipModeEnabled = false;
            e.Handled = true;
        }
    }
    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, IsMinimized ? "Minimized" : "Expanded", true);
        VisualStateManager.GoToState(this, _overflowCommands.Count > 0 ? "Overflow" : "NoOverflow", true);
    }
}

internal sealed class CommandRibbonAutomationPeer(CommandRibbon owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ToolBar;
    protected override string GetClassNameCore() => nameof(CommandRibbon);
    protected override string GetNameCore() => AutomationProperties.GetName(Owner) is { Length: > 0 } name ? name : "Command ribbon";
    protected override bool IsKeyboardFocusableCore() => Owner is CommandRibbon ribbon && ribbon.IsEnabled && ribbon.IsTabStop;
}
