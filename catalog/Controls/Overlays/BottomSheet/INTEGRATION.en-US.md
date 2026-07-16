# BottomSheet Integration

## Host and contracts

One or more BottomSheets may live at a window content root, but only one per Z layer may be Open/Opening. A later sheet is above and becomes the Escape target. It consumes Theme, Motion, Input, Accessibility, and a window-level overlay coordinator. It never creates an `AppWindow`.

All calls run on the UI thread. Place the sheet as the final child of a full-client-area Grid, not inside a ScrollViewer or clipped container. Unload cleans it up without migration to another window.

## Platform, lifecycle, and degradation

Translation uses `Microsoft.UI.Composition`, input uses pointer capture, and focus uses WinUI FocusManager. No network, files, capabilities, or external APIs are required. Window deactivation retains Open state while stopping animation and releasing capture; activation remeasures.

Unavailable Composition or disabled system animation applies final layout immediately. If focus containment cannot be established, modal open throws `InvalidOperationException`, avoiding a visually modal surface with interactive background.
