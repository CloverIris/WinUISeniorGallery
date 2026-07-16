# CompactOverlayHost Design

## Visual Hierarchy and Responsive Behavior

Candidate is nonvisual; optional OverlayChromePresenter shows exit, topmost explanation, and minimum controls only. Never duplicate page content. Narrow/regular/wide and 100%–300% DPI share one state model; Window change never rebuilds host data and 200% text uses reflow/overflow.

## Input and Focus

Overlay always has keyboard/touch/mouse exit; host policy owns Escape and it never closes Window; focus remains in visible content.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not depend on material/transparency. Reduced Motion uses instant/fade, targets are 44×44 epx, and Automation exposes window state, action, and error.

## Modernization Tradeoffs

Archaeology owns provenance; never pixel-copy system Shell and preserve system menu, Snap, Caption Buttons, and reserved gestures.

