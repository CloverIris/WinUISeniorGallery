# Game Bar Widget Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Preserve: An overlay that does not interrupt foreground content, independent widget window state, pin/unpin, compact and desktop modes, and continuous 2D focus navigation across gamepad and mouse.

## Modernize

Reconstruct as an in-app floating-widget experience: the host supplies a workspace and widgets can move, resize, minimize, and stay on top. It never impersonates system Game Bar or injects into other processes. Use current Fluent ThemeResources, corners, typography, and system icons; historical proportions and motion are research references, not pixel copies.

## Responsive Behavior and Input

Verify D-pad/stick+A/B, mouse, keyboard, and touch; focus never lands on obscured controls and pinned state retains an exit path. Narrow windows fall back to one column or a full-screen layer; large screens cap content width and retain 10-foot legibility.

## Motion and Accessibility

Motion conveys entry, exit, and state change only; Reduced Motion switches instantly. High Contrast does not rely on transparency, screen readers receive role, state, and next action, and focus paths are reversible.
