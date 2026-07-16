# ConnectedTransitionBehavior Design

## Visual Hierarchy and Responsive Behavior

No fixed template; candidate layers are source snapshot/proxy Visual, target placeholder, and transition mask. Hide real target only when ready and always restore visibility. Narrow/regular/wide and 100%–300% DPI share one state model; Window change never rebuilds host data and 200% text uses reflow/overflow.

## Input and Focus

Proxy Visual never gets focus/hit testing; navigation target owns focus after completion. Back may use reverse configuration without blocking.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not depend on material/transparency. Reduced Motion uses instant/fade, targets are 44×44 epx, and Automation exposes window state, action, and error.

## Modernization Tradeoffs

Archaeology owns provenance; never pixel-copy system Shell and preserve system menu, Snap, Caption Buttons, and reserved gestures.

