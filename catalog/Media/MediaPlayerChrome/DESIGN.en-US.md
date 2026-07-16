# MediaPlayerChrome Design

## Visuals

The default is a low-distraction Fluent chrome with a bottom gradient scrim, 44×44 epx minimum targets, and the play button at the highest hierarchy. `Full` shows all actions, `Compact` moves infrequent actions into overflow, and `Minimal` retains play, time, and an entry action.

## Interaction

The host owns click-to-toggle behavior on the video surface; the control handles input only inside its visual tree. Space/gamepad A toggles play, arrow keys seek, J/L skip backward/forward, M mutes, F requests full screen, and Escape requests leaving full screen. Shortcuts are not intercepted in text input or menu focus.

## Responsive Behavior

- Width ≥ 720 epx: `Full`.
- 360–719 epx: `Compact`.
- < 360 epx: `Minimal`.

An explicit `DisplayMode` wins and templates never assume a fixed window width. Compact overlay retains play/pause, progress, volume entry, and a request to return Inline.

## Theme and Resources

Use only `ThemeResource`. Stable keys are `MediaPlayerChromeBackgroundBrush`, `MediaPlayerChromeForegroundBrush`, `MediaPlayerChromeSecondaryForegroundBrush`, `MediaPlayerChromeControlFillBrush`, `MediaPlayerChromeErrorBrush`, `MediaPlayerChromeCornerRadius`, `MediaPlayerChromeControlSpacing`, and `MediaPlayerChromeAutoHideDuration`.

Light, Dark, and High Contrast remain equivalently legible. When video reduces contrast, use a gradient scrim instead of a hard-coded black surface.

## Motion

Chrome uses a 160 ms fade-in and 200 ms fade-out with slight vertical movement. Buffering uses non-blocking progress motion only. With Reduced Motion enabled, remove translation and scale, retain instant visibility changes and essential progress feedback.

## Input and Accessibility

Support mouse, touch, pen, keyboard, touchpad, and gamepad. Order is play, stop, timeline, volume, rate, window mode, and more. Buttons have localized names; the play button names the next action. Buffering is not announced repeatedly; errors and presentation changes use polite LiveRegion announcements.
