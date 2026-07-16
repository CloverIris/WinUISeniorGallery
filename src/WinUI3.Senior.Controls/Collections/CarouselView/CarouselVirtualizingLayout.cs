using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUI3.Senior.Controls;

/// <summary>
/// Realizes only the selected source item and its configured source-index buffer.
/// Looping maps indexes; it never creates a duplicated item source.
/// </summary>
public sealed class CarouselVirtualizingLayout : VirtualizingLayout
{
    private readonly Dictionary<int, UIElement> _realizedElements = [];

    public int SelectedIndex { get; set; }

    public int RealizationBuffer { get; set; } = 1;

    public CarouselNavigationMode NavigationMode { get; set; }

    internal int RealizedElementCount => _realizedElements.Count;

    internal event EventHandler? RealizedElementsChanged;

    protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
    {
        var desiredIndexes = GetDesiredIndexes(context.ItemCount);
        RecycleUndesiredElements(context, desiredIndexes);

        foreach (var index in desiredIndexes)
        {
            var element = context.GetOrCreateElementAt(index);
            _realizedElements[index] = element;
            element.Measure(availableSize);
        }

        RaiseRealizedElementsChanged();

        return availableSize;
    }

    protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
    {
        foreach (var pair in _realizedElements)
        {
            var offset = CarouselIndexMapper.RelativeOffset(pair.Key, SelectedIndex, context.ItemCount, NavigationMode);
            pair.Value.Arrange(new Rect(offset * finalSize.Width, 0, finalSize.Width, finalSize.Height));
        }

        return finalSize;
    }

    protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
    {
        RecycleAll(context);
        base.UninitializeForContextCore(context);
    }

    private HashSet<int> GetDesiredIndexes(int itemCount)
    {
        var indexes = new HashSet<int>();
        if (itemCount <= 0)
        {
            return indexes;
        }

        var selectedIndex = CarouselIndexMapper.Normalize(SelectedIndex, itemCount, NavigationMode);
        if (selectedIndex < 0)
        {
            selectedIndex = 0;
        }

        for (var offset = -RealizationBuffer; offset <= RealizationBuffer; offset++)
        {
            var index = CarouselIndexMapper.Normalize(selectedIndex + offset, itemCount, NavigationMode);
            if (index >= 0)
            {
                indexes.Add(index);
            }
        }

        return indexes;
    }

    private void RecycleUndesiredElements(VirtualizingLayoutContext context, HashSet<int> desiredIndexes)
    {
        var obsoleteIndexes = new List<int>();
        foreach (var pair in _realizedElements)
        {
            if (!desiredIndexes.Contains(pair.Key))
            {
                context.RecycleElement(pair.Value);
                obsoleteIndexes.Add(pair.Key);
            }
        }

        foreach (var index in obsoleteIndexes)
        {
            _realizedElements.Remove(index);
        }
    }

    private void RecycleAll(VirtualizingLayoutContext context)
    {
        foreach (var element in _realizedElements.Values)
        {
            context.RecycleElement(element);
        }

        _realizedElements.Clear();
        RaiseRealizedElementsChanged();
    }

    internal UIElement? GetRealizedElement(int index) =>
        _realizedElements.TryGetValue(index, out var element) ? element : null;

    internal void RefreshLayout() => InvalidateMeasure();

    private void RaiseRealizedElementsChanged() => RealizedElementsChanged?.Invoke(this, EventArgs.Empty);
}
