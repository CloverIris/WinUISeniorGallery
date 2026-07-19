using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace WinUI3.Senior.Controls;

public sealed record GuideNode(string Id, string Label, object? Icon = null, IReadOnlyList<GuideNode>? Children = null, object? Tag = null)
{
    public GuideNode Normalize() => string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Label)
        ? throw new ArgumentException("Guide nodes require non-empty Id and Label.")
        : this with { Children = (Children ?? Array.Empty<GuideNode>()).Select(child => child.Normalize()).ToArray() };
}

public sealed class GuideNodeInvokedEventArgs(GuideNode node, bool hasChildren) : EventArgs
{
    public GuideNode Node { get; } = node ?? throw new ArgumentNullException(nameof(node));
    public bool HasChildren { get; } = hasChildren;
}

[TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
[TemplatePart(Name = "PART_Scrim", Type = typeof(Border))]
[TemplatePart(Name = "PART_Breadcrumb", Type = typeof(ItemsControl))]
[TemplatePart(Name = "PART_Nodes", Type = typeof(ItemsControl))]
public sealed class GuideMenuExperience : Control
{
    private readonly ObservableCollection<GuideNode> _nodes = new();
    private readonly ObservableCollection<GuideNode> _currentItems = new();
    private readonly List<GuideNode> _path = new();
    private ItemsControl? _nodePart;
    private ItemsControl? _breadcrumbPart;

    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(GuideMenuExperience), new PropertyMetadata(false));
    public static readonly DependencyProperty IsExecutingProperty = DependencyProperty.Register(nameof(IsExecuting), typeof(bool), typeof(GuideMenuExperience), new PropertyMetadata(false));
    public static readonly DependencyProperty IsDismissOnLeafInvokeProperty = DependencyProperty.Register(nameof(IsDismissOnLeafInvoke), typeof(bool), typeof(GuideMenuExperience), new PropertyMetadata(false));

    public GuideMenuExperience()
    {
        DefaultStyleKey = typeof(GuideMenuExperience);
        KeyDown += OnKeyDown;
        _nodes.CollectionChanged += (_, _) => ResetToRoot();
    }

    public ObservableCollection<GuideNode> Nodes => _nodes;
    public IReadOnlyList<GuideNode> CurrentItems => _currentItems;
    public IReadOnlyList<GuideNode> NavigationPath => _path;
    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public bool IsExecuting { get => (bool)GetValue(IsExecutingProperty); private set => SetValue(IsExecutingProperty, value); }
    public bool IsDismissOnLeafInvoke { get => (bool)GetValue(IsDismissOnLeafInvokeProperty); set => SetValue(IsDismissOnLeafInvokeProperty, value); }
    public event EventHandler<GuideNodeInvokedEventArgs>? NodeInvoked;
    public event EventHandler? Closed;
    public event EventHandler? NavigationChanged;

    public void SetNodes(IEnumerable<GuideNode> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        var normalized = nodes.Select(node => node.Normalize()).GroupBy(node => node.Id, StringComparer.Ordinal).Select(group => group.First()).ToArray();
        _nodes.Clear();
        foreach (var node in normalized) _nodes.Add(node);
        ResetToRoot();
    }

    public bool Open() { IsOpen = _nodes.Count > 0; ResetToRoot(); return IsOpen; }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        IsExecuting = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    public bool NavigateBack()
    {
        if (_path.Count == 0) return false;
        _path.RemoveAt(_path.Count - 1);
        RefreshCurrentItems();
        NavigationChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool Invoke(string id)
    {
        var node = _currentItems.FirstOrDefault(item => string.Equals(item.Id, id, StringComparison.Ordinal));
        if (node is null || !IsOpen) return false;
        if (node.Children is { Count: > 0 })
        {
            _path.Add(node);
            RefreshCurrentItems();
            NavigationChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (IsDismissOnLeafInvoke)
        {
            IsExecuting = true;
            NodeInvoked?.Invoke(this, new GuideNodeInvokedEventArgs(node, false));
            Close();
        }
        else
        {
            NodeInvoked?.Invoke(this, new GuideNodeInvokedEventArgs(node, false));
        }
        return true;
    }

    protected override void OnApplyTemplate()
    {
        if (_nodePart is not null) _nodePart.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnNodeClick));
        base.OnApplyTemplate();
        _nodePart = GetTemplateChild("PART_Nodes") as ItemsControl;
        _breadcrumbPart = GetTemplateChild("PART_Breadcrumb") as ItemsControl;
        if (_nodePart is not null) _nodePart.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnNodeClick), true);
        RefreshCurrentItems();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    private void ResetToRoot()
    {
        _path.Clear();
        RefreshCurrentItems();
        NavigationChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshCurrentItems()
    {
        _currentItems.Clear();
        var items = _path.Count == 0 ? _nodes : _path[^1].Children ?? Array.Empty<GuideNode>();
        foreach (var item in items) _currentItems.Add(item);
        if (_nodePart is not null) _nodePart.ItemsSource = _currentItems;
        if (_breadcrumbPart is not null) _breadcrumbPart.ItemsSource = _path;
    }

    private void OnNodeClick(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement { Tag: string id }) Invoke(id);
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Escape)
        {
            if (!NavigateBack()) Close();
            e.Handled = true;
        }
    }
}
