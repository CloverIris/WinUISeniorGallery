# Property Grid Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Selection-driven reflective editing, separation of metadata and editors, common properties for multi-selection, inline validation, and host-transaction undo.

## Modernize

Reconstruct as an extensible PropertyGrid: hosts supply descriptors, editor factories, and change transactions; the control owns categories, filtering, virtualization, validation, and keyboard navigation. Appearance uses current Fluent ThemeResources, system typography, and modern focus visuals instead of copying classic-dialog pixels.

## Responsive Behavior and Input

Tab moves among name/value/editor, arrows expand hierarchy, Enter starts/commits, and Escape rolls back; each row exposes name, type, value, read-only, and validation state. Narrow layouts use collapse, overflow, or one column and wide layouts cap content width; 200% text scaling never clips commit/cancel paths.

## Motion and Accessibility

State transitions last at most 200 ms and Reduced Motion switches instantly. High Contrast uses system colors and non-color cues; Automation exposes structure, state, validation, and next action.
