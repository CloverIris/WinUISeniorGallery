# MediaCenterGrid Design

## Visual Hierarchy

Default 16:9/2:3 posters, 10-foot text, and clear focus ring; scale does not obscure neighbors and narrow windows become a rail.

## Responsive Behavior

Narrow/regular/wide share one state model; Window change never rebuilds data and 200% text scaling uses reflow/overflow.

## Input and Focus

Gamepad/D-pad first, A/Enter invokes and B/Escape returns; mouse/wheel/touch/keyboard equivalent. Focus never enters unrealized items and Automation exposes row/column, title, actions.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not rely on transparency. Reduced Motion switches instantly, targets are 44×44 epx, and Automation exposes state/action/error.

## Modernization Tradeoffs

Archaeology owns provenance; use modern Fluent/RTL/multi-input without copying assets/system appearance.

