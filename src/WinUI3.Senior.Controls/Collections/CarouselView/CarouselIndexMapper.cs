using System;

namespace WinUI3.Senior.Controls;

/// <summary>Pure source-index operations used by <see cref="CarouselView"/> and its layout.</summary>
internal static class CarouselIndexMapper
{
    internal static int Normalize(int index, int itemCount, CarouselNavigationMode navigationMode)
    {
        if (itemCount <= 0)
        {
            return -1;
        }

        if (navigationMode == CarouselNavigationMode.Bounded)
        {
            return index is >= 0 and < int.MaxValue && index < itemCount ? index : -1;
        }

        var remainder = index % itemCount;
        return remainder < 0 ? remainder + itemCount : remainder;
    }

    internal static int Next(int currentIndex, int itemCount, CarouselNavigationMode navigationMode)
    {
        if (itemCount <= 0)
        {
            return -1;
        }

        if (currentIndex < 0)
        {
            return 0;
        }

        if (currentIndex == itemCount - 1)
        {
            return navigationMode == CarouselNavigationMode.Loop ? 0 : currentIndex;
        }

        return currentIndex + 1;
    }

    internal static int Previous(int currentIndex, int itemCount, CarouselNavigationMode navigationMode)
    {
        if (itemCount <= 0)
        {
            return -1;
        }

        if (currentIndex < 0)
        {
            return 0;
        }

        if (currentIndex == 0)
        {
            return navigationMode == CarouselNavigationMode.Loop ? itemCount - 1 : 0;
        }

        return currentIndex - 1;
    }

    internal static int RelativeOffset(int sourceIndex, int selectedIndex, int itemCount, CarouselNavigationMode navigationMode)
    {
        if (itemCount <= 0 || selectedIndex < 0)
        {
            return 0;
        }

        var offset = sourceIndex - selectedIndex;
        if (navigationMode != CarouselNavigationMode.Loop)
        {
            return offset;
        }

        var half = itemCount / 2;
        if (offset > half)
        {
            offset -= itemCount;
        }
        else if (offset < -half)
        {
            offset += itemCount;
        }

        return offset;
    }
}
