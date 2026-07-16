# BottomSheet Design

## Visuals and motion

Bottom placement rounds top corners; Side leaves the window-edge side square; Center rounds all corners. Surface uses system Layer Fill and themed shadow. Modal scrim defaults to 32% black and is theme-overridable. The handle is at least 32×4 px visually with a 44×24 px hit target.

Open and snap use Composition translation targeting 280 ms; close targets 220 ms. Velocity seeds spring motion but never skips the target snap point. Reduced Motion removes spring translation, uses at most 100 ms fade, and completes snapping immediately.

## Responsiveness, theme, and resources

Public keys: `BottomSheetSurfaceBackground`, `BottomSheetScrimBrush`, `BottomSheetCornerRadius`, `BottomSheetShadow`, `BottomSheetDragHandleBrush`, `BottomSheetOpenDuration`, `BottomSheetCloseDuration`, `BottomSheetMinExtent`. Light/Dark/Mica/Acrylic hosts consume system resources. High Contrast disables shadow and translucent scrim in favor of system Window/Text colors and a 2 px border.

Soft keyboard, title-bar inset, taskbar, and display cutouts reduce available bounds. Oversized content scrolls in the template's ScrollViewer; the sheet never leaves the window.

## Accessibility

The modal AutomationPeer exposes Window/Transform patterns with `IsModal=true`; modeless is a Pane. Snap changes announce readable names; continuous drag pixels do not. The handle supports Enter/Space cycling, arrow-key snap movement, and Escape dismissal.
