# SnackbarHost Design

## Visuals, layout, and motion

Default placement is BottomCenter, 24 px from client safe edges, max width 480 px and min width 288 px; below 336 px it keeps 16 px side margins. Content is icon, wrapping message, one text action, and optional dismiss button. Visual text is capped at three lines while Automation Name keeps the full message.

Show uses 16 px translation plus 180 ms fade; close is 120 ms; same-key in-place replacement cross-fades for 100 ms. Reduced Motion keeps at most a 100 ms fade. High Contrast removes shadow and translucency.

## Theme resources

Public keys: `SnackbarHostMaxWidth`, `SnackbarHostMargin`, `SnackbarSurfaceBackground`, `SnackbarForeground`, `SnackbarBorderBrush`, `SnackbarCornerRadius`, `SnackbarShadow`, `SnackbarActionButtonStyle`, `SnackbarShowDuration`, `SnackbarCloseDuration`, plus icon/accent keys for four Kinds. All use ThemeResource. Error never relies on color alone.

## Input and accessibility

Appearance does not steal focus; Action is reachable by Tab. Ordinary messages use a polite live region; Warning/Error/Critical are assertive. Non-empty `AutomationAnnouncement` replaces Message for announcement. In-place dedup updates announce new text once.

Touch, mouse, and keyboard Action/Dismiss targets are at least 44×44 px. RTL mirrors icon, action, and dismiss order. Timeout pauses while screen reader or keyboard focus is inside.
