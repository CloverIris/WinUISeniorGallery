# CarouselView Specification

## Goals and non-goals

The control must remain finitely virtualized with 1000 logical items, handle mouse, wheel, keyboard, touch, precision touchpad, and controller input consistently, and provide `Slide`, `Fade`, and `CoverFlow` transitions. It does not fetch data, use networking, cache images, navigate pages, play media, or recreate historical Cover Flow assets.

## Public API

```csharp
public enum CarouselTransition { Slide, Fade, CoverFlow }
public enum CarouselNavigationMode { Bounded, Loop }
public enum CarouselAutoplayPauseReason { None, PointerOver, KeyboardFocusWithin, WindowInactive, UserInteraction, NotVisible }
public enum CarouselInputDeviceKind { Unknown, Mouse, Touch, Keyboard, GameController, PrecisionTouchpad }

public sealed class CarouselView : ListView
{
    public CarouselNavigationMode NavigationMode { get; set; } // Bounded
    public CarouselTransition Transition { get; set; } // Slide
    public bool IsAutoplayEnabled { get; set; } // false
    public bool IsHostWindowActive { get; set; } // true; host supplied
    public TimeSpan AutoplayInterval { get; set; } // 5 s; minimum 1 s
    public bool PauseAutoplayOnPointerOver { get; set; } // true
    public bool PauseAutoplayOnKeyboardFocusWithin { get; set; } // true
    public int RealizationBuffer { get; set; } // 1; range 0..3
    public bool IsAdjacentPreviewEnabled { get; set; } // false
    public double AdjacentPreviewExtent { get; set; } // 48 effective px; non-negative
    public bool IsSwipeEnabled { get; set; } // true
    public ICommand? ItemInvokedCommand { get; set; }
    public CarouselAutoplayPauseReason AutoplayPauseReason { get; }
    public int RealizedElementCount { get; } // Lab diagnostic, not a layout guarantee
    public new object? SelectedItem { get; }
    public event EventHandler<CarouselItemInvokedEventArgs> ItemInvoked;
    public void MoveNext();
    public void MovePrevious();
    public void MoveTo(int index);
}
```

Inherited `ItemsSource`, `ItemTemplate`, `ItemTemplateSelector`, `SelectedIndex`, `SelectionChanged`, and `IsSwipeEnabled` retain `ListView`/`Selector` semantics. Properties and methods are UI-thread-only. `CarouselItemInvokedEventArgs` permanently exposes `Item`, `Index`, and `InputDeviceKind`; when a precision touchpad cannot be identified reliably, it must report `Unknown`.

`MoveTo` is a no-op for an empty collection and throws `ArgumentOutOfRangeException` for an out-of-range index in a non-empty collection. `AutoplayInterval < 1s`, `RealizationBuffer` outside 0..3, and negative `AdjacentPreviewExtent` throw `ArgumentOutOfRangeException`. `IsHostWindowActive` defaults to `true`, is written only by the host, and never queries, activates, or creates a window.

## State model

States are `Empty`, `Idle`, `Interacting`, `Settling`, `AutoplayWaiting`, and `Suspended`. The autoplay pause precedence is fixed as `NotVisible > WindowInactive > KeyboardFocusWithin > PointerOver > UserInteraction > None`. Any pause ending or user navigation stops the timer and waits a full `AutoplayInterval` before another move.

Runtime replacements of `ItemsSource`, `ItemTemplate`, and `ItemTemplateSelector` must synchronize to the internal `ItemsRepeater` without enumerating or duplicating all data. Collection changes retain selection by object identity first; when that object was removed, select the successor at the same position, or the new final item after final-item removal; an empty collection selects `-1`, and a non-empty unselected collection selects `0`.

## Template parts and visual tree

- `PART_Root`: root `Grid`.
- `PART_Repeater`: required `ItemsRepeater` content host.
- `PART_PreviousButton`, `PART_NextButton`: optional navigation buttons.
- `PART_IndicatorPresenter`: optional indicator host.
- `PART_LiveRegion`: optional screen-reader announcement element.

The template must supply `CommonStates` (`Normal`, `PointerOver`, `Disabled`), `NavigationStates` (`CanPrevious`, `CanNext`, `SingleItem`), and `InteractionStates` (`Idle`, `Dragging`, `Settling`). Missing optional parts only degrade behaviour; a missing `PART_Repeater` throws a clear `InvalidOperationException`.

## Behaviour and failure modes

Arrow keys and controller D-pad navigate in visual direction; RTL mirrors direction. Home/End go to first/last; Enter/Space/controller A invoke the selected item. Horizontal wheel or Shift+vertical wheel navigates while ordinary vertical wheel bubbles. A drag commits only past 20% width or 500 effective pixels/second release velocity; otherwise it settles back. Dragging, animation, and settling are `UserInteraction`.

When Reduced Motion is enabled or Composition is unavailable, `Slide`/`CoverFlow` degrade to a fade of at most 100ms or no animation. A new navigation interrupts active animation and converges from the current visual value. Autoplay must not issue Live Region announcements; only user selection changes announce. An `ItemsSource` enumeration failure propagates its original exception.
