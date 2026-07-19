namespace WinUI3.Senior.Controls;

/// <summary>Immutable layout metrics that can be shown in diagnostics without inspecting visuals.</summary>
public sealed record AdaptiveGridLayoutResult(
    double AvailableWidth,
    int ItemCount,
    int ColumnCount,
    int RowCount,
    double ItemWidth,
    double HorizontalSpacing,
    double VerticalSpacing)
{
    public double ContentWidth => ColumnCount <= 0 ? 0 : ColumnCount * ItemWidth + Math.Max(0, ColumnCount - 1) * HorizontalSpacing;
}

public sealed class AdaptiveGridLayoutChangedEventArgs(AdaptiveGridLayoutResult layout) : EventArgs
{
    public AdaptiveGridLayoutResult Layout { get; } = layout ?? throw new ArgumentNullException(nameof(layout));
}

/// <summary>
/// Shared deterministic column calculation for AdaptiveGrid, HomeScreen and dashboard hosts.
/// It does not measure or reparent XAML children.
/// </summary>
public static class AdaptiveGridLayoutCalculator
{
    public static AdaptiveGridLayoutResult Calculate(
        double availableWidth,
        int itemCount,
        double minimumItemWidth = 180,
        double maximumItemWidth = double.PositiveInfinity,
        double horizontalSpacing = 16,
        double verticalSpacing = 16,
        int maxColumns = 0)
    {
        var width = double.IsFinite(availableWidth) ? Math.Max(0, availableWidth) : Math.Max(1, minimumItemWidth);
        var minimum = double.IsFinite(minimumItemWidth) ? Math.Max(1, minimumItemWidth) : 180;
        var maximum = double.IsFinite(maximumItemWidth) ? Math.Max(minimum, maximumItemWidth) : double.PositiveInfinity;
        var hGap = double.IsFinite(horizontalSpacing) ? Math.Max(0, horizontalSpacing) : 0;
        var vGap = double.IsFinite(verticalSpacing) ? Math.Max(0, verticalSpacing) : 0;
        var count = Math.Max(0, itemCount);
        var columns = Math.Max(1, (int)Math.Floor((Math.Max(width, minimum) + hGap) / (minimum + hGap)));
        if (maxColumns > 0) columns = Math.Min(columns, maxColumns);
        if (count > 0) columns = Math.Min(columns, count);
        var itemWidth = Math.Max(1, (Math.Max(width, minimum) - (columns - 1) * hGap) / columns);
        itemWidth = Math.Min(itemWidth, maximum);
        var rows = count == 0 ? 0 : (int)Math.Ceiling(count / (double)columns);
        return new AdaptiveGridLayoutResult(width, count, columns, rows, itemWidth, hGap, vGap);
    }
}
