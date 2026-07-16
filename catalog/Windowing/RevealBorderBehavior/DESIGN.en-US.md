# RevealBorderBehavior Design

## Visual Hierarchy and Responsive Behavior

No template parts; candidate Composition tree is per-window shared light plus per-element border mask. High Contrast/Reduced Motion may disable light while retaining standard border. Narrow/regular/wide and 100%–300% DPI share one state model; Window change never rebuilds host data and 200% text uses reflow/overflow.

## Input and Focus

Mouse/pen hover gets light and touch press feedback; keyboard/gamepad always has independent standard FocusVisual and never relies on Reveal.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not depend on material/transparency. Reduced Motion uses instant/fade, targets are 44×44 epx, and Automation exposes window state, action, and error.

## Modernization Tradeoffs

Archaeology owns provenance; never pixel-copy system Shell and preserve system menu, Snap, Caption Buttons, and reserved gestures.

