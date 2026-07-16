# Tree Data Grid Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Hierarchy expansion in the first column, aligned comparison columns, full-row selection, column sort/resize, deep virtualization, and keyboard tree navigation.

## Modernize

Reconstruct as a data-source-driven TreeDataGrid: hosts provide stable node IDs, asynchronous children, and columns; the control owns viewport flattening, virtualization, selection, sort requests, and expansion state. Appearance uses current Fluent ThemeResources, system typography, and modern focus visuals instead of copying classic-dialog pixels.

## Responsive Behavior and Input

Left/right collapses/expands or traverses hierarchy, up/down moves visible rows, and Home/End/Page navigate; screen readers receive row/column index, level, expansion, and selection. Narrow layouts use collapse, overflow, or one column and wide layouts cap content width; 200% text scaling never clips commit/cancel paths.

## Motion and Accessibility

State transitions last at most 200 ms and Reduced Motion switches instantly. High Contrast uses system colors and non-color cues; Automation exposes structure, state, validation, and next action.
