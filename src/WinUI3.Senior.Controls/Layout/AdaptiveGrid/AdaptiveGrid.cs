using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUI3.Senior.Controls;

/// <summary>A responsive panel that computes columns from available width without reparenting children.</summary>
public sealed class AdaptiveGrid : Panel
{
    public static readonly DependencyProperty MinimumItemWidthProperty = DependencyProperty.Register(nameof(MinimumItemWidth), typeof(double), typeof(AdaptiveGrid), new PropertyMetadata(180d, OnLayoutPropertyChanged));
    public static readonly DependencyProperty MaximumItemWidthProperty = DependencyProperty.Register(nameof(MaximumItemWidth), typeof(double), typeof(AdaptiveGrid), new PropertyMetadata(double.PositiveInfinity, OnLayoutPropertyChanged));
    public static readonly DependencyProperty HorizontalSpacingProperty = DependencyProperty.Register(nameof(HorizontalSpacing), typeof(double), typeof(AdaptiveGrid), new PropertyMetadata(16d, OnLayoutPropertyChanged));
    public static readonly DependencyProperty VerticalSpacingProperty = DependencyProperty.Register(nameof(VerticalSpacing), typeof(double), typeof(AdaptiveGrid), new PropertyMetadata(16d, OnLayoutPropertyChanged));
    public static readonly DependencyProperty MaxColumnsProperty = DependencyProperty.Register(nameof(MaxColumns), typeof(int), typeof(AdaptiveGrid), new PropertyMetadata(0, OnLayoutPropertyChanged));
    public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register(nameof(ColumnCount), typeof(int), typeof(AdaptiveGrid), new PropertyMetadata(0));

    public double MinimumItemWidth { get => (double)GetValue(MinimumItemWidthProperty); set => SetValue(MinimumItemWidthProperty, double.IsFinite(value) ? Math.Max(1, value) : 180d); }
    public double MaximumItemWidth { get => (double)GetValue(MaximumItemWidthProperty); set => SetValue(MaximumItemWidthProperty, double.IsFinite(value) ? Math.Max(1, value) : double.PositiveInfinity); }
    public double HorizontalSpacing { get => (double)GetValue(HorizontalSpacingProperty); set => SetValue(HorizontalSpacingProperty, double.IsFinite(value) ? Math.Max(0, value) : 0d); }
    public double VerticalSpacing { get => (double)GetValue(VerticalSpacingProperty); set => SetValue(VerticalSpacingProperty, double.IsFinite(value) ? Math.Max(0, value) : 0d); }
    public int MaxColumns { get => (int)GetValue(MaxColumnsProperty); set => SetValue(MaxColumnsProperty, Math.Max(0, value)); }
    public int ColumnCount => (int)GetValue(ColumnCountProperty);
    public AdaptiveGridLayoutResult? CurrentLayout { get; private set; }
    public event EventHandler<AdaptiveGridLayoutChangedEventArgs>? LayoutChanged;

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width) ? SafeMinimumItemWidth : Math.Max(availableSize.Width, SafeMinimumItemWidth);
        var columns = ComputeColumns(width);
        var itemWidth = Math.Max(1, (width - (columns - 1) * SafeHorizontalSpacing) / columns);
        itemWidth = Math.Min(itemWidth, SafeMaximumItemWidth);
        var rowHeight = 0d; var totalHeight = 0d; var col = 0;
        foreach (var child in Children)
        {
            child.Measure(new Size(itemWidth, double.PositiveInfinity));
            rowHeight = Math.Max(rowHeight, child.DesiredSize.Height); col++;
            if (col == columns) { totalHeight += rowHeight; if (col < Children.Count) totalHeight += SafeVerticalSpacing; col = 0; rowHeight = 0; }
        }
        if (col > 0) totalHeight += rowHeight;
        SetValue(ColumnCountProperty, columns);
        PublishLayout(width);
        return new Size(double.IsInfinity(availableSize.Width) ? columns * itemWidth + (columns - 1) * SafeHorizontalSpacing : availableSize.Width, totalHeight);
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        var columns = Math.Max(1, ComputeColumns(finalSize.Width)); var itemWidth = Math.Max(1, (finalSize.Width - (columns - 1) * SafeHorizontalSpacing) / columns); itemWidth = Math.Min(itemWidth, SafeMaximumItemWidth);
        var x = 0d; var y = 0d; var rowHeight = 0d; var col = 0;
        foreach (var child in Children)
        {
            var height = child.DesiredSize.Height; child.Arrange(new Rect(x, y, itemWidth, height)); rowHeight = Math.Max(rowHeight, height); col++;
            if (col == columns) { y += rowHeight + SafeVerticalSpacing; x = 0; col = 0; rowHeight = 0; } else x += itemWidth + SafeHorizontalSpacing;
        }
        SetValue(ColumnCountProperty, columns);
        PublishLayout(finalSize.Width);
        return finalSize;
    }
    private void PublishLayout(double width)
    {
        var layout = AdaptiveGridLayoutCalculator.Calculate(width, Children.Count, MinimumItemWidth, MaximumItemWidth, HorizontalSpacing, VerticalSpacing, MaxColumns);
        if (Equals(CurrentLayout, layout)) return;
        CurrentLayout = layout;
        LayoutChanged?.Invoke(this, new AdaptiveGridLayoutChangedEventArgs(layout));
    }
    private int ComputeColumns(double width)
    {
        var min = SafeMinimumItemWidth; var columns = Math.Max(1, (int)Math.Floor((Math.Max(0, width) + SafeHorizontalSpacing) / (min + SafeHorizontalSpacing)));
        if (MaxColumns > 0) columns = Math.Min(columns, MaxColumns); return Math.Min(columns, Math.Max(1, Children.Count == 0 ? columns : Children.Count));
    }
    private double SafeMinimumItemWidth => double.IsFinite(MinimumItemWidth) ? Math.Max(1, MinimumItemWidth) : 180d;
    private double SafeMaximumItemWidth => double.IsFinite(MaximumItemWidth) ? Math.Max(SafeMinimumItemWidth, MaximumItemWidth) : double.PositiveInfinity;
    private double SafeHorizontalSpacing => Math.Max(0, HorizontalSpacing);
    private double SafeVerticalSpacing => Math.Max(0, VerticalSpacing);
    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((AdaptiveGrid)d).InvalidateMeasure();
}
