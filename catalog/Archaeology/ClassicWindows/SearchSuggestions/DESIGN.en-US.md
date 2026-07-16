# Search Suggestions Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Feedback while typing, source grouping, keyboard preview selection, distinction between query and suggestion commit, and explicit no-results feedback.

## Modernize

Reconstruct as SearchBoxEx: hosts inject cancellable multi-source Providers and the control merges groups, recents, and discovery entries; it never uploads queries or accesses Windows Search index by default. Appearance uses current Fluent ThemeResources, system typography, and modern focus visuals instead of copying classic-dialog pixels.

## Responsive Behavior and Input

Arrows browse, Enter commits, and Escape closes suggestions then clears by level; touch and screen readers expose category, source, and result type. Narrow layouts use collapse, overflow, or one column and wide layouts cap content width; 200% text scaling never clips commit/cancel paths.

## Motion and Accessibility

State transitions last at most 200 ms and Reduced Motion switches instantly. High Contrast uses system colors and non-color cues; Automation exposes structure, state, validation, and next action.
