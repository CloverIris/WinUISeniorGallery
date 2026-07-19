using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3.Senior.Controls;

public sealed record HubSection(string Id, string Header, IReadOnlyList<object> Items, object? Background = null, object? Tag = null)
{
    public HubSection Normalize()
    {
        if (string.IsNullOrWhiteSpace(Id)) throw new ArgumentException("A section id is required.", nameof(Id));
        if (string.IsNullOrWhiteSpace(Header)) throw new ArgumentException("A section header is required.", nameof(Header));
        return this with { Items = Items ?? Array.Empty<object>() };
    }
}

public sealed class HubSectionChangedEventArgs(HubSection? previous, HubSection? current, int index, bool isUserInitiated) : EventArgs
{
    public HubSection? Previous { get; } = previous;
    public HubSection? Current { get; } = current;
    public int Index { get; } = index;
    public bool IsUserInitiated { get; } = isUserInitiated;
}

/// <summary>
/// Horizontal panorama composition inspired by Metro Hub. It owns section selection and
/// scroll coordination; content, navigation, and background assets remain host-owned.
/// </summary>
[TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
[TemplatePart(Name = "PART_Repeater", Type = typeof(ItemsRepeater))]
[TemplatePart(Name = "PART_Indicator", Type = typeof(ListView))]
public sealed class HubPanorama : Control
{
    private readonly ObservableCollection<HubSection> _sections = new();
    private ScrollViewer? _scrollViewer;
    private ItemsRepeater? _repeater;
    private ListView? _indicator;
    private bool _internalScroll;

    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(HubPanorama), new PropertyMetadata(-1, OnSelectionChanged));
    public static readonly DependencyProperty SectionWidthProperty = DependencyProperty.Register(nameof(SectionWidth), typeof(double), typeof(HubPanorama), new PropertyMetadata(720d));
    public static readonly DependencyProperty IsParallaxEnabledProperty = DependencyProperty.Register(nameof(IsParallaxEnabled), typeof(bool), typeof(HubPanorama), new PropertyMetadata(true));
    public static readonly DependencyProperty ParallaxStrengthProperty = DependencyProperty.Register(nameof(ParallaxStrength), typeof(double), typeof(HubPanorama), new PropertyMetadata(.18));
    public static readonly DependencyProperty IsWrapNavigationEnabledProperty = DependencyProperty.Register(nameof(IsWrapNavigationEnabled), typeof(bool), typeof(HubPanorama), new PropertyMetadata(true));
    public static readonly DependencyProperty IsReducedMotionProperty = DependencyProperty.Register(nameof(IsReducedMotion), typeof(bool), typeof(HubPanorama), new PropertyMetadata(false));

    public HubPanorama()
    {
        DefaultStyleKey = typeof(HubPanorama);
        _sections.CollectionChanged += (_, _) => SyncSections();
        KeyDown += OnKeyDown;
        IsTabStop = true;
    }

    public ObservableCollection<HubSection> Sections => _sections;
    public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); private set => SetValue(SelectedIndexProperty, value); }
    public HubSection? SelectedSection => SelectedIndex >= 0 && SelectedIndex < _sections.Count ? _sections[SelectedIndex] : null;
    public double SectionWidth { get => (double)GetValue(SectionWidthProperty); set => SetValue(SectionWidthProperty, Math.Clamp(value, 240, 2000)); }
    public bool IsParallaxEnabled { get => (bool)GetValue(IsParallaxEnabledProperty); set => SetValue(IsParallaxEnabledProperty, value); }
    public double ParallaxStrength { get => (double)GetValue(ParallaxStrengthProperty); set => SetValue(ParallaxStrengthProperty, Math.Clamp(value, 0, .8)); }
    public bool IsWrapNavigationEnabled { get => (bool)GetValue(IsWrapNavigationEnabledProperty); set => SetValue(IsWrapNavigationEnabledProperty, value); }
    public bool IsReducedMotion { get => (bool)GetValue(IsReducedMotionProperty); set => SetValue(IsReducedMotionProperty, value); }
    public event EventHandler<HubSectionChangedEventArgs>? SectionChanged;

    public void SetSections(IEnumerable<HubSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);
        var normalized = sections.Select(section => section.Normalize()).GroupBy(section => section.Id, StringComparer.Ordinal).Select(group => group.First()).ToArray();
        _sections.Clear();
        foreach (var section in normalized) _sections.Add(section);
        SelectedIndex = _sections.Count == 0 ? -1 : Math.Clamp(SelectedIndex < 0 ? 0 : SelectedIndex, 0, _sections.Count - 1);
        SyncSections();
    }

    public bool SelectSection(int index, bool isUserInitiated = true)
    {
        if (index < 0 || index >= _sections.Count) return false;
        var previous = SelectedSection;
        if (SelectedIndex == index)
        {
            ScrollToIndex(index, false);
            return true;
        }
        SelectedIndex = index;
        ScrollToIndex(index, !IsReducedMotion);
        SectionChanged?.Invoke(this, new HubSectionChangedEventArgs(previous, _sections[index], index, isUserInitiated));
        return true;
    }

    public bool Navigate(int delta, bool isUserInitiated = true)
    {
        if (_sections.Count == 0 || delta == 0) return false;
        var next = (SelectedIndex < 0 ? 0 : SelectedIndex) + Math.Sign(delta);
        if (IsWrapNavigationEnabled) next = (next % _sections.Count + _sections.Count) % _sections.Count;
        else next = Math.Clamp(next, 0, _sections.Count - 1);
        return SelectSection(next, isUserInitiated);
    }

    protected override void OnApplyTemplate()
    {
        if (_scrollViewer is not null) _scrollViewer.ViewChanged -= OnViewChanged;
        if (_indicator is not null) _indicator.SelectionChanged -= OnIndicatorSelectionChanged;
        base.OnApplyTemplate();
        _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
        _repeater = GetTemplateChild("PART_Repeater") as ItemsRepeater;
        _indicator = GetTemplateChild("PART_Indicator") as ListView;
        if (_scrollViewer is not null) _scrollViewer.ViewChanged += OnViewChanged;
        if (_indicator is not null) _indicator.SelectionChanged += OnIndicatorSelectionChanged;
        SyncSections();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panorama = (HubPanorama)d;
        panorama.SyncIndicator();
    }

    private void SyncSections()
    {
        if (_repeater is not null) _repeater.ItemsSource = _sections;
        if (_indicator is not null) _indicator.ItemsSource = _sections;
        SyncIndicator();
        if (SelectedIndex >= 0) ScrollToIndex(SelectedIndex, false);
    }

    private void SyncIndicator()
    {
        if (_indicator is not null) _indicator.SelectedIndex = SelectedIndex;
    }

    private void ScrollToIndex(int index, bool animated)
    {
        if (_scrollViewer is null) return;
        _internalScroll = true;
        try { _scrollViewer.ChangeView(index * SectionWidth, null, null, disableAnimation: !animated); }
        finally { _internalScroll = false; }
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (_internalScroll || _sections.Count == 0 || SectionWidth <= 0) return;
        var index = Math.Clamp((int)Math.Round((_scrollViewer?.HorizontalOffset ?? 0) / SectionWidth), 0, _sections.Count - 1);
        if (index != SelectedIndex) SelectSection(index, true);
    }

    private void OnIndicatorSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || _indicator is null) return;
        SelectSection(_indicator.SelectedIndex, true);
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var direction = e.Key switch
        {
            Windows.System.VirtualKey.Left => FlowDirection == FlowDirection.RightToLeft ? 1 : -1,
            Windows.System.VirtualKey.Right => FlowDirection == FlowDirection.RightToLeft ? -1 : 1,
            Windows.System.VirtualKey.Home => -100000,
            Windows.System.VirtualKey.End => 100000,
            _ => 0,
        };
        if (direction == -100000) e.Handled = SelectSection(0);
        else if (direction == 100000) e.Handled = SelectSection(_sections.Count - 1);
        else if (direction != 0) e.Handled = Navigate(direction);
    }
}
