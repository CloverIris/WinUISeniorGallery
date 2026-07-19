using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace WinUI3.Senior.Controls;

public enum OverlayMenuModality
{
    Modal,
    NonModal,
}

public enum OverlayMenuPlacement
{
    Right,
    Left,
    Bottom,
    Center,
}

public sealed class OverlayMenuItem
{
    public OverlayMenuItem(string id, string header, object? icon = null, object? tag = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("An item id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(header)) throw new ArgumentException("An item header is required.", nameof(header));
        Id = id; Header = header; Icon = icon; Tag = tag;
    }

    public string Id { get; }
    public string Header { get; }
    public object? Icon { get; }
    public object? Tag { get; }
    public bool IsEnabled { get; set; } = true;
    public ObservableCollection<OverlayMenuItem> Children { get; } = new();
    public bool HasChildren => Children.Count > 0;
}

public sealed class OverlayMenuItemInvokedEventArgs(OverlayMenuItem item, bool enteredSubmenu) : EventArgs
{
    public OverlayMenuItem Item { get; } = item ?? throw new ArgumentNullException(nameof(item));
    public bool EnteredSubmenu { get; } = enteredSubmenu;
    public bool Handled { get; set; }
}

public sealed class OverlayMenuNavigationChangedEventArgs(IReadOnlyList<OverlayMenuItem> path) : EventArgs
{
    public IReadOnlyList<OverlayMenuItem> Path { get; } = path ?? throw new ArgumentNullException(nameof(path));
    public OverlayMenuItem? Current => Path.Count == 0 ? null : Path[Path.Count - 1];
}

[TemplatePart(Name = "PART_Scrim", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_Panel", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_Items", Type = typeof(ItemsControl))]
[TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
public sealed class OverlayMenu : Control
{
    private readonly ObservableCollection<OverlayMenuItem> _items = new();
    private readonly ObservableCollection<OverlayMenuItem> _visibleItems = new();
    private readonly Stack<OverlayMenuItem> _path = new();
    private ItemsControl? _itemsPart;
    private Button? _backButton;
    private FrameworkElement? _scrim;
    private FrameworkElement? _panel;

    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(OverlayMenu), new PropertyMetadata(false, OnOpenChanged));
    public static readonly DependencyProperty ModalityProperty = DependencyProperty.Register(nameof(Modality), typeof(OverlayMenuModality), typeof(OverlayMenu), new PropertyMetadata(OverlayMenuModality.Modal, OnMenuVisualPropertyChanged));
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(nameof(Placement), typeof(OverlayMenuPlacement), typeof(OverlayMenu), new PropertyMetadata(OverlayMenuPlacement.Right, OnMenuVisualPropertyChanged));
    public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.Register(nameof(IsBackButtonVisible), typeof(bool), typeof(OverlayMenu), new PropertyMetadata(true, OnMenuVisualPropertyChanged));

    public OverlayMenu()
    {
        DefaultStyleKey = typeof(OverlayMenu);
        _items.CollectionChanged += (_, _) =>
        {
            if (IsOpen && _path.Count == 0) UpdateItems(_items);
        };
        KeyDown += OnKeyDown;
    }

    public ObservableCollection<OverlayMenuItem> Items => _items;
    public IReadOnlyList<OverlayMenuItem> CurrentItems => _visibleItems;
    public IReadOnlyList<OverlayMenuItem> NavigationPath => _path.Reverse().ToArray();
    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public OverlayMenuModality Modality { get => (OverlayMenuModality)GetValue(ModalityProperty); set => SetValue(ModalityProperty, value); }
    public OverlayMenuPlacement Placement { get => (OverlayMenuPlacement)GetValue(PlacementProperty); set => SetValue(PlacementProperty, value); }
    public bool IsBackButtonVisible { get => (bool)GetValue(IsBackButtonVisibleProperty); set => SetValue(IsBackButtonVisibleProperty, value); }

    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public event EventHandler<OverlayMenuItemInvokedEventArgs>? ItemInvoked;
    public event EventHandler<OverlayMenuNavigationChangedEventArgs>? NavigationChanged;

    public void Open()
    {
        if (IsOpen) return;
        _path.Clear();
        UpdateItems(_items);
        IsOpen = true;
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
    }

    public bool NavigateBack()
    {
        if (_path.Count == 0) return false;
        _path.Pop();
        UpdateItems(_path.Count == 0 ? _items : _path.Peek().Children);
        return true;
    }

    public bool Invoke(string id)
    {
        var item = _visibleItems.FirstOrDefault(candidate => string.Equals(candidate.Id, id, StringComparison.Ordinal));
        if (item is null || !item.IsEnabled) return false;
        if (item.HasChildren)
        {
            _path.Push(item);
            UpdateItems(item.Children);
            ItemInvoked?.Invoke(this, new OverlayMenuItemInvokedEventArgs(item, true));
            return true;
        }
        var args = new OverlayMenuItemInvokedEventArgs(item, false);
        ItemInvoked?.Invoke(this, args);
        if (!args.Handled && Modality == OverlayMenuModality.Modal) Close();
        return true;
    }

    protected override void OnApplyTemplate()
    {
        if (_scrim is not null) _scrim.PointerPressed -= OnScrimPressed;
        if (_backButton is not null) _backButton.Click -= OnBackClick;
        if (_itemsPart is not null) _itemsPart.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnItemButtonClick));
        base.OnApplyTemplate();
        _scrim = GetTemplateChild("PART_Scrim") as FrameworkElement;
        _panel = GetTemplateChild("PART_Panel") as FrameworkElement;
        _itemsPart = GetTemplateChild("PART_Items") as ItemsControl;
        _backButton = GetTemplateChild("PART_BackButton") as Button;
        if (_scrim is not null) _scrim.PointerPressed += OnScrimPressed;
        if (_backButton is not null) _backButton.Click += OnBackClick;
        if (_itemsPart is not null) _itemsPart.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnItemButtonClick), true);
        UpdatePanelPlacement();
        UpdateItems(_path.Count == 0 ? _items : _path.Peek().Children);
        UpdateVisualState();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    private static void OnOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var menu = (OverlayMenu)d;
        menu.UpdateVisualState();
        if ((bool)e.NewValue) menu.Opened?.Invoke(menu, EventArgs.Empty); else menu.Closed?.Invoke(menu, EventArgs.Empty);
    }

    private static void OnMenuVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var menu = (OverlayMenu)d;
        menu.UpdateVisualState();
        menu.UpdatePanelPlacement();
        menu.UpdateItems(menu._path.Count == 0 ? menu._items : menu._path.Peek().Children);
    }

    private void UpdateItems(IEnumerable<OverlayMenuItem> items)
    {
        _visibleItems.Clear();
        foreach (var item in items) _visibleItems.Add(item);
        if (_itemsPart is not null) _itemsPart.ItemsSource = _visibleItems;
        if (_backButton is not null) { _backButton.Visibility = IsBackButtonVisible && _path.Count > 0 ? Visibility.Visible : Visibility.Collapsed; _backButton.IsEnabled = _path.Count > 0; }
        NavigationChanged?.Invoke(this, new OverlayMenuNavigationChangedEventArgs(NavigationPath));
    }

    private void UpdateVisualState()
    {
        if (_scrim is not null) _scrim.Visibility = IsOpen && Modality == OverlayMenuModality.Modal ? Visibility.Visible : Visibility.Collapsed;
        if (_panel is not null) _panel.Visibility = IsOpen ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdatePanelPlacement()
    {
        if (_panel is null) return;
        _panel.HorizontalAlignment = Placement switch
        {
            OverlayMenuPlacement.Left => HorizontalAlignment.Left,
            OverlayMenuPlacement.Center => HorizontalAlignment.Center,
            OverlayMenuPlacement.Bottom => HorizontalAlignment.Stretch,
            _ => HorizontalAlignment.Right,
        };
        _panel.VerticalAlignment = Placement == OverlayMenuPlacement.Bottom ? VerticalAlignment.Bottom : VerticalAlignment.Center;
        _panel.ClearValue(FrameworkElement.WidthProperty);
        if (Placement != OverlayMenuPlacement.Bottom && Placement != OverlayMenuPlacement.Center) _panel.Width = 360;
    }

    private void OnScrimPressed(object sender, PointerRoutedEventArgs e) { if (Modality == OverlayMenuModality.Modal) Close(); e.Handled = true; }
    private void OnBackClick(object sender, RoutedEventArgs e) => NavigateBack();
    private void OnItemButtonClick(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is Button { Tag: string id })
        {
            Invoke(id);
            e.Handled = true;
        }
    }
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (!IsOpen) return;
        if (e.Key == VirtualKey.Escape)
        {
            if (_path.Count > 0)
            {
                NavigateBack();
                e.Handled = true;
            }
            else
            {
                e.Handled = Modality == OverlayMenuModality.Modal;
                if (e.Handled) Close();
            }
        }
    }
}
