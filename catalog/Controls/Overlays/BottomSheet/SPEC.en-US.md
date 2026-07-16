# BottomSheet Specification

## Goals and non-goals

Provide a one-sheet host at a window content root with modal/modeless modes, pixel/fraction/content snap points, drag, scrim dismissal, Escape, focus containment, and narrow/wide adaptive placement. App routing, cross-window migration, nested sheets, and system-wide edge gestures are excluded.

## Public API

```csharp
public enum BottomSheetModality { Modal, Modeless }
public enum BottomSheetPlacement { Auto, Bottom, Side, Center }
public enum BottomSheetDismissReason { Programmatic, Drag, Scrim, Escape, Back, HostUnloaded }
public enum BottomSheetSnapPointKind { Pixels, AvailableFraction, Content }
public sealed record BottomSheetSnapPoint(string Id, BottomSheetSnapPointKind Kind, double Value);

public sealed class BottomSheet : ContentControl
{
    public bool IsOpen { get; set; }
    public BottomSheetModality Modality { get; set; } // Modal
    public BottomSheetPlacement Placement { get; set; } // Auto
    public IReadOnlyList<BottomSheetSnapPoint> SnapPoints { get; set; }
    public string? ActiveSnapPointId { get; set; }
    public bool IsDragEnabled { get; set; } // true
    public bool IsDismissOnScrimEnabled { get; set; } // true when Modal
    public bool IsDismissOnEscapeEnabled { get; set; } // true
    public bool IsFocusTrapEnabled { get; set; } // true when Modal
    public double WideModeThreshold { get; set; } // 720
    public BottomSheetPlacement WideModePlacement { get; set; } // Side
    public event EventHandler<BottomSheetOpeningEventArgs> Opening;
    public event EventHandler<EventArgs> Opened;
    public event EventHandler<BottomSheetClosingEventArgs> Closing;
    public event EventHandler<BottomSheetClosedEventArgs> Closed;
    public event EventHandler<BottomSheetSnapPointChangedEventArgs> SnapPointChanged;
    public void Open(string? snapPointId = null);
    public void Close(BottomSheetDismissReason reason = Programmatic);
    public void SnapTo(string snapPointId);
}
```

`Opening` and `Closing` are cancellable and cancellation restores the prior state. `Closing` supplies the reason. The default is a `Content` point bounded by 50% available height. IDs are non-empty and unique; pixel values are positive and fractions are in `(0,1]`, otherwise assignment throws `ArgumentException`. Unknown `SnapTo` IDs throw `ArgumentException`.

## State and layout

Lifecycle: `Closed → Opening → Open → Snapping/Dragging → Closing → Closed`. Animations are interruptible: Close during Opening transitions from current visual values, and vice versa. Exactly one terminal event fires.

`Auto` is Bottom below `WideModeThreshold`, otherwise `WideModePlacement`. Side follows flow direction (right in LTR, left in RTL); Center is dialog-like. Resizing preserves the logical snap-point ID and clamps it to the new safe area.

Snap extent is calculated from available space and clamped from 64 px to safe-area maximum. Release velocity over 800 px/s selects the adjacent point in that direction; otherwise nearest point wins. Dragging 25% below the minimum or closing velocity over 1,000 px/s dismisses.

## Template parts

- `PART_Root`: overlay root.
- `PART_Scrim`: scrim.
- `PART_Surface`: sheet surface and transform target.
- `PART_DragHandle`: optional handle.
- `PART_ContentPresenter`: content host.
- `PART_FocusSentinelStart`, `PART_FocusSentinelEnd`: modal focus loop.

Visual state groups are `OpenStates`, `PlacementStates`, `ModalityStates`, and `InteractionStates`. Missing Surface or ContentPresenter raises a template exception; other omissions degrade safely.

## Focus and dismissal

Modal open saves current window focus and moves to the first focusable child, or Surface. Tab cycles inside; background is temporarily excluded from the interactive UIA view. Close restores the prior element when valid, otherwise the host. Modeless does not trap focus or hide background, and outside clicks do not close by default.

Escape closes only the topmost open BottomSheet after descendants can handle it. A cancelled Closing consumes the key. Host unload forces non-cancellable `HostUnloaded` closure and releases capture and focus sentinels.
