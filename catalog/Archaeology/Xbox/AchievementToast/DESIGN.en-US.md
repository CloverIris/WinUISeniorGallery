# Achievement Toast Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Preserve: High-recognition feedback for rare events, short non-modal presentation, strict queuing, one highlighted achievement at a time, and cumulative progress available in details.

## Modernize

Reconstruct as a general achievement toast: the host supplies localized title, icon, progress, and rarity; support queueing, coalescing, timeout, and motion-free mode without copying Xbox audio or graphics. Use current Fluent ThemeResources, corners, typography, and system icons; historical proportions and motion are research references, not pixel copies.

## Responsive Behavior and Input

It takes no focus by default; if an action exists, keyboard/gamepad enters explicitly, and Escape/B returns focus to the original host. Narrow windows fall back to one column or a full-screen layer; large screens cap content width and retain 10-foot legibility.

## Motion and Accessibility

Motion conveys entry, exit, and state change only; Reduced Motion switches instantly. High Contrast does not rely on transparency, screen readers receive role, state, and next action, and focus paths are reversible.
