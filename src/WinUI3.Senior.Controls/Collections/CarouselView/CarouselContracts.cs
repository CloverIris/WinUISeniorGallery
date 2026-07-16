using System;
using Microsoft.UI.Xaml;

namespace WinUI3.Senior.Controls;

/// <summary>Specifies the visual transition requested for a <see cref="CarouselView"/>.</summary>
public enum CarouselTransition
{
    Slide,
    Fade,
    CoverFlow,
}

/// <summary>Specifies whether navigation stops at the collection boundaries.</summary>
public enum CarouselNavigationMode
{
    Bounded,
    Loop,
}

/// <summary>Identifies the highest-priority condition currently pausing autoplay.</summary>
public enum CarouselAutoplayPauseReason
{
    None,
    PointerOver,
    KeyboardFocusWithin,
    WindowInactive,
    UserInteraction,
    NotVisible,
}

/// <summary>Identifies the device that committed a carousel item invocation.</summary>
public enum CarouselInputDeviceKind
{
    Unknown,
    Mouse,
    Touch,
    Keyboard,
    GameController,
    PrecisionTouchpad,
}

/// <summary>Provides the item, source index, and input device for an item invocation.</summary>
public sealed class CarouselItemInvokedEventArgs : EventArgs
{
    public CarouselItemInvokedEventArgs(object? item, int index, CarouselInputDeviceKind inputDeviceKind)
    {
        Item = item;
        Index = index;
        InputDeviceKind = inputDeviceKind;
    }

    public object? Item { get; }

    public int Index { get; }

    public CarouselInputDeviceKind InputDeviceKind { get; }
}
