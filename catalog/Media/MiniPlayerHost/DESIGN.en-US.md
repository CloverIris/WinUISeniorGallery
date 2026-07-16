# MiniPlayerHost Design

## Visual Hierarchy

Mini mode prioritizes artwork/thumbnail, title, play/pause, and expand; target at least 280×72 epx, otherwise icon strip.

## Responsive Behavior

Narrow/regular/wide share one state model; Window change never rebuilds data and 200% text scaling uses reflow/overflow.

## Input and Focus

Collapse focuses the nearest semantic mini element and expand restores focus; Escape does not collapse and drag works only in DragRegion.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not rely on transparency. Reduced Motion switches instantly, targets are 44×44 epx, and Automation exposes state/action/error.

## Modernization Tradeoffs

Archaeology owns provenance; use modern Fluent/RTL/multi-input without copying assets/system appearance.

