# Editable Breadcrumb Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Persistent current location, direct ancestor access, collapse of early levels under constrained width, and coexistence of visual navigation with exact text entry.

## Modernize

Reconstruct as BreadcrumbBarEx, adding edit mode, history/autocomplete, hierarchy menus, and optional drag/drop to the base BreadcrumbBar; path parsing and navigation belong to a host Provider. Appearance uses current Fluent ThemeResources, system typography, and modern focus visuals instead of copying classic-dialog pixels.

## Responsive Behavior and Input

F4/Ctrl+L enters editing, Enter commits, and Escape cancels; mouse, touch, keyboard, and screen reader can expand collapsed levels. Narrow layouts use collapse, overflow, or one column and wide layouts cap content width; 200% text scaling never clips commit/cancel paths.

## Motion and Accessibility

State transitions last at most 200 ms and Reduced Motion switches instantly. High Contrast uses system colors and non-color cues; Automation exposes structure, state, validation, and next action.
