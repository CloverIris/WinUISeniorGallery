namespace WinUI3.Senior.Controls;

/// <summary>Pure lifecycle model for timing and interaction. It deliberately owns no XAML objects.</summary>
internal sealed class CarouselStateMachine
{
    internal CarouselOperationalState State { get; private set; } = CarouselOperationalState.Empty;

    internal CarouselAutoplayPauseReason PauseReason { get; private set; } = CarouselAutoplayPauseReason.NotVisible;

    internal void Update(int itemCount, bool isLoaded, bool isVisible, bool isWindowActive, bool pointerOver, bool keyboardFocusWithin, bool userInteraction)
    {
        PauseReason = GetPauseReason(isVisible, isWindowActive, keyboardFocusWithin, pointerOver, userInteraction);
        State = itemCount == 0
            ? CarouselOperationalState.Empty
            : userInteraction
                ? CarouselOperationalState.Interacting
                : PauseReason != CarouselAutoplayPauseReason.None || !isLoaded || itemCount <= 1
                    ? CarouselOperationalState.Suspended
                    : CarouselOperationalState.AutoplayWaiting;
    }

    internal static CarouselAutoplayPauseReason GetPauseReason(bool isVisible, bool isWindowActive, bool keyboardFocusWithin, bool pointerOver, bool userInteraction)
    {
        // The order is contractual. Do not reorder these tests without changing the specification.
        if (!isVisible)
        {
            return CarouselAutoplayPauseReason.NotVisible;
        }

        if (!isWindowActive)
        {
            return CarouselAutoplayPauseReason.WindowInactive;
        }

        if (keyboardFocusWithin)
        {
            return CarouselAutoplayPauseReason.KeyboardFocusWithin;
        }

        if (pointerOver)
        {
            return CarouselAutoplayPauseReason.PointerOver;
        }

        return userInteraction ? CarouselAutoplayPauseReason.UserInteraction : CarouselAutoplayPauseReason.None;
    }
}

internal enum CarouselOperationalState
{
    Empty,
    Idle,
    Interacting,
    Settling,
    AutoplayWaiting,
    Suspended,
}
