# SettingsPanel Design

## Visual Hierarchy and Responsive Behavior

Candidate tree: Scrim, PaneRoot, Header, BackButton, FramePresenter, Footer, FocusSentinels, ErrorPresenter. Narrow/regular/wide and 100%–300% DPI share one state model; Window change never rebuilds host data and 200% text uses reflow/overflow.

## Input and Focus

Alt+Left/Backspace/B navigates back and Escape closes inner popup then panel; modal focus is contained, close restores trigger, errors focus first invalid field.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not depend on material/transparency. Reduced Motion uses instant/fade, targets are 44×44 epx, and Automation exposes window state, action, and error.

## Modernization Tradeoffs

Archaeology owns provenance; never pixel-copy system Shell and preserve system menu, Snap, Caption Buttons, and reserved gestures.

