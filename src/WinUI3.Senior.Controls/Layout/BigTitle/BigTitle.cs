using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WinUI3.Senior.Controls;

public enum BigTitleState
{
    Expanded,
    Collapsing,
    Collapsed,
}

/// <summary>
/// Scroll-aware large title that collapses into a compact title without owning navigation.
/// </summary>
[TemplatePart(Name = "PART_TitlePresenter", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_CompactPresenter", Type = typeof(TextBlock))]
public sealed class BigTitle : Control
{
    private TextBlock? _titlePresenter;
    private TextBlock? _compactPresenter;
    private ScrollViewer? _scrollSource;

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(BigTitle), new PropertyMetadata(string.Empty, OnVisualPropertyChanged));
    public static readonly DependencyProperty ExpandedFontSizeProperty = DependencyProperty.Register(nameof(ExpandedFontSize), typeof(double), typeof(BigTitle), new PropertyMetadata(64d, OnVisualPropertyChanged));
    public static readonly DependencyProperty CollapsedFontSizeProperty = DependencyProperty.Register(nameof(CollapsedFontSize), typeof(double), typeof(BigTitle), new PropertyMetadata(20d, OnVisualPropertyChanged));
    public static readonly DependencyProperty CollapseDistanceProperty = DependencyProperty.Register(nameof(CollapseDistance), typeof(double), typeof(BigTitle), new PropertyMetadata(120d));
    public static readonly DependencyProperty CollapseProgressProperty = DependencyProperty.Register(nameof(CollapseProgress), typeof(double), typeof(BigTitle), new PropertyMetadata(0d, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsStickyProperty = DependencyProperty.Register(nameof(IsSticky), typeof(bool), typeof(BigTitle), new PropertyMetadata(true));
    public static readonly DependencyProperty ScrollSourceProperty = DependencyProperty.Register(nameof(ScrollSource), typeof(ScrollViewer), typeof(BigTitle), new PropertyMetadata(null, OnScrollSourceChanged));

    public BigTitle()
    {
        DefaultStyleKey = typeof(BigTitle);
        Loaded += (_, _) => { _scrollSource = ScrollSource; AttachScrollSource(); ApplyVisuals(); };
        Unloaded += (_, _) => DetachScrollSource();
    }

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value ?? string.Empty); }
    public double ExpandedFontSize { get => (double)GetValue(ExpandedFontSizeProperty); set => SetValue(ExpandedFontSizeProperty, Math.Clamp(value, 20, 160)); }
    public double CollapsedFontSize { get => (double)GetValue(CollapsedFontSizeProperty); set => SetValue(CollapsedFontSizeProperty, Math.Clamp(value, 10, 80)); }
    public double CollapseDistance { get => (double)GetValue(CollapseDistanceProperty); set => SetValue(CollapseDistanceProperty, Math.Max(1, value)); }
    public double CollapseProgress { get => (double)GetValue(CollapseProgressProperty); private set => SetValue(CollapseProgressProperty, Math.Clamp(value, 0, 1)); }
    public bool IsSticky { get => (bool)GetValue(IsStickyProperty); set => SetValue(IsStickyProperty, value); }
    public ScrollViewer? ScrollSource { get => (ScrollViewer?)GetValue(ScrollSourceProperty); set => SetValue(ScrollSourceProperty, value); }
    public BigTitleState State => CollapseProgress <= 0 ? BigTitleState.Expanded : CollapseProgress >= 1 ? BigTitleState.Collapsed : BigTitleState.Collapsing;
    public event EventHandler? StateChanged;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _titlePresenter = GetTemplateChild("PART_TitlePresenter") as TextBlock;
        _compactPresenter = GetTemplateChild("PART_CompactPresenter") as TextBlock;
        ApplyVisuals();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BigTitle)d).ApplyVisuals();
    private static void OnScrollSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var title = (BigTitle)d;
        title.DetachScrollSource();
        title._scrollSource = e.NewValue as ScrollViewer;
        title.AttachScrollSource();
        title.UpdateProgress();
    }

    private void DetachScrollSource()
    {
        if (_scrollSource is not null) _scrollSource.ViewChanged -= OnScrollChanged;
        _scrollSource = null;
    }

    private void AttachScrollSource()
    {
        if (_scrollSource is not null) _scrollSource.ViewChanged += OnScrollChanged;
        UpdateProgress();
    }

    private void OnScrollChanged(object? sender, ScrollViewerViewChangedEventArgs e) => UpdateProgress();
    private void UpdateProgress()
    {
        var next = _scrollSource is null ? 0 : Math.Clamp(_scrollSource.VerticalOffset / CollapseDistance, 0, 1);
        var previous = CollapseProgress;
        CollapseProgress = next;
        if (Math.Abs(previous - next) > double.Epsilon) { ApplyVisuals(); StateChanged?.Invoke(this, EventArgs.Empty); }
    }

    private void ApplyVisuals()
    {
        if (_titlePresenter is null && _compactPresenter is null) return;
        var progress = Math.Clamp(CollapseProgress, 0, 1);
        if (_titlePresenter is not null)
        {
            _titlePresenter.Text = Text;
            _titlePresenter.FontSize = ExpandedFontSize + (CollapsedFontSize - ExpandedFontSize) * progress;
            _titlePresenter.Opacity = 1 - progress;
            _titlePresenter.RenderTransform = new TranslateTransform { Y = -24 * progress };
        }
        if (_compactPresenter is not null)
        {
            _compactPresenter.Text = Text;
            _compactPresenter.FontSize = CollapsedFontSize;
            _compactPresenter.Opacity = progress;
        }
    }
}
