using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace WinUI3.Senior.Controls;

public sealed record MixNode(string Id, string Title, string Kind = "Related", IReadOnlyList<string>? RelatedIds = null, object? Content = null, object? Tag = null)
{
    public MixNode Normalize() => string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Title)
        ? throw new ArgumentException("Mix nodes require non-empty Id and Title.")
        : this with { RelatedIds = RelatedIds ?? Array.Empty<string>() };
}

public sealed class MixNodeSelectedEventArgs(MixNode node, bool isUserInitiated) : EventArgs
{
    public MixNode Node { get; } = node ?? throw new ArgumentNullException(nameof(node));
    public bool IsUserInitiated { get; } = isUserInitiated;
}

[TemplatePart(Name = "PART_Surface", Type = typeof(Canvas))]
[TemplatePart(Name = "PART_LiveRegion", Type = typeof(TextBlock))]
public sealed class MixviewExperience : Control
{
    private readonly ObservableCollection<MixNode> _nodes = new();
    private Canvas? _surface;
    private TextBlock? _liveRegion;

    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(MixviewExperience), new PropertyMetadata(false, OnVisualPropertyChanged));
    public static readonly DependencyProperty RootNodeIdProperty = DependencyProperty.Register(nameof(RootNodeId), typeof(string), typeof(MixviewExperience), new PropertyMetadata(null, OnVisualPropertyChanged));
    public static readonly DependencyProperty SelectedNodeIdProperty = DependencyProperty.Register(nameof(SelectedNodeId), typeof(string), typeof(MixviewExperience), new PropertyMetadata(null, OnVisualPropertyChanged));
    public static readonly DependencyProperty MaxVisibleNodesProperty = DependencyProperty.Register(nameof(MaxVisibleNodes), typeof(int), typeof(MixviewExperience), new PropertyMetadata(12, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsReducedMotionProperty = DependencyProperty.Register(nameof(IsReducedMotion), typeof(bool), typeof(MixviewExperience), new PropertyMetadata(false));

    public MixviewExperience()
    {
        DefaultStyleKey = typeof(MixviewExperience);
        _nodes.CollectionChanged += (_, _) => RenderGraph();
        KeyDown += OnKeyDown;
        SizeChanged += (_, _) => RenderGraph();
    }

    public ObservableCollection<MixNode> Nodes => _nodes;
    public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public string? RootNodeId { get => (string?)GetValue(RootNodeIdProperty); set => SetValue(RootNodeIdProperty, value); }
    public string? SelectedNodeId { get => (string?)GetValue(SelectedNodeIdProperty); private set => SetValue(SelectedNodeIdProperty, value); }
    public int MaxVisibleNodes { get => (int)GetValue(MaxVisibleNodesProperty); set => SetValue(MaxVisibleNodesProperty, Math.Clamp(value, 1, 64)); }
    public bool IsReducedMotion { get => (bool)GetValue(IsReducedMotionProperty); set => SetValue(IsReducedMotionProperty, value); }
    public MixNode? SelectedNode => Find(SelectedNodeId ?? RootNodeId);
    public event EventHandler<MixNodeSelectedEventArgs>? NodeSelected;
    public event EventHandler? Closed;

    public void SetNodes(IEnumerable<MixNode> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        var normalized = nodes.Select(node => node.Normalize()).GroupBy(node => node.Id, StringComparer.Ordinal).Select(group => group.First()).ToArray();
        _nodes.Clear();
        foreach (var node in normalized) _nodes.Add(node);
        if (RootNodeId is null || Find(RootNodeId) is null) RootNodeId = _nodes.FirstOrDefault()?.Id;
        if (SelectedNodeId is null || Find(SelectedNodeId) is null) SelectedNodeId = RootNodeId;
        IsOpen = _nodes.Count > 0;
        RenderGraph();
    }

    public bool Open(string? nodeId = null)
    {
        var node = Find(nodeId ?? RootNodeId);
        if (node is null) return false;
        SelectedNodeId = node.Id;
        IsOpen = true;
        RenderGraph();
        return true;
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        RenderGraph();
        Closed?.Invoke(this, EventArgs.Empty);
    }

    public bool SelectNode(string id, bool isUserInitiated = true)
    {
        var node = Find(id);
        if (node is null) return false;
        SelectedNodeId = node.Id;
        IsOpen = true;
        NodeSelected?.Invoke(this, new MixNodeSelectedEventArgs(node, isUserInitiated));
        if (_liveRegion is not null) _liveRegion.Text = $"Selected {node.Title}";
        RenderGraph();
        return true;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _surface = GetTemplateChild("PART_Surface") as Canvas;
        _liveRegion = GetTemplateChild("PART_LiveRegion") as TextBlock;
        RenderGraph();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);
    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MixviewExperience)d).RenderGraph();
    private MixNode? Find(string? id) => string.IsNullOrWhiteSpace(id) ? null : _nodes.FirstOrDefault(node => string.Equals(node.Id, id, StringComparison.Ordinal));

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Escape) { Close(); e.Handled = true; }
    }

    private void RenderGraph()
    {
        if (_surface is null) return;
        _surface.Children.Clear();
        _surface.Visibility = IsOpen ? Visibility.Visible : Visibility.Collapsed;
        var center = new Windows.Foundation.Point(Math.Max(120, ActualWidth / 2), Math.Max(100, ActualHeight / 2));
        var selected = SelectedNode ?? Find(RootNodeId);
        if (selected is null) return;
        AddNodeButton(selected, center, true);
        var related = selected.RelatedIds?.Select(Find).Where(node => node is not null).Cast<MixNode>().Take(MaxVisibleNodes).ToArray() ?? Array.Empty<MixNode>();
        if (related.Length == 0) related = _nodes.Where(node => !string.Equals(node.Id, selected.Id, StringComparison.Ordinal)).Take(MaxVisibleNodes).ToArray();
        for (var index = 0; index < related.Length; index++)
        {
            var angle = (Math.PI * 2 * index / Math.Max(1, related.Length)) - Math.PI / 2;
            AddNodeButton(related[index], new Windows.Foundation.Point(center.X + Math.Cos(angle) * 220, center.Y + Math.Sin(angle) * 140), false);
        }
    }

    private void AddNodeButton(MixNode node, Windows.Foundation.Point point, bool selected)
    {
        if (_surface is null) return;
        var button = new Button { Content = node.Title, Tag = node.Id, MinWidth = selected ? 150 : 110, MinHeight = selected ? 64 : 48, Opacity = selected ? 1 : .9, Background = new SolidColorBrush(Windows.UI.Color.FromArgb(selected ? (byte)255 : (byte)210, 55, 90, 130)) };
        AutomationProperties.SetName(button, $"Mixview node {node.Title}");
        button.Click += OnNodeButtonClick;
        Canvas.SetLeft(button, point.X - button.MinWidth / 2);
        Canvas.SetTop(button, point.Y - button.MinHeight / 2);
        _surface.Children.Add(button);
    }

    private void OnNodeButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string id }) SelectNode(id);
    }
}
