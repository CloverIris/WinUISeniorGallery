# TitleBarHost Design

## Visual Hierarchy and Responsive Behavior

Candidate tree: RootGrid, IconPresenter, TextStack, Left/Center/RightPresenter, DragSurface, CaptionInsetSpacer. Interactive content is automatically excluded from drag. Narrow/regular/wide and 100%–300% DPI share one state model; Window change never rebuilds host data and 200% text uses reflow/overflow.

## Input and Focus

Tab enters interactive content only, never drag background; preserve caption order, RTL, Alt+Space, and title-bar double-click.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not depend on material/transparency. Reduced Motion uses instant/fade, targets are 44×44 epx, and Automation exposes window state, action, and error.

## Modernization Tradeoffs

Archaeology owns provenance; never pixel-copy system Shell and preserve system menu, Snap, Caption Buttons, and reserved gestures.

