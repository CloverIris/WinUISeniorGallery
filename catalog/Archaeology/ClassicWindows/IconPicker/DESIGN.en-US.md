# Icon Picker Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Grid browsing of large sets, explicit current selection, switchable source path, separation of preview and resource index, and keyboard type-ahead.

## Modernize

Reconstruct as a safe IconPicker supporting app-registered Symbol/IconSource catalogs, search, categories, favorites, and RTL metadata; it does not scan arbitrary DLL/EXE files or extract system resources by default. Appearance uses current Fluent ThemeResources, system typography, and modern focus visuals instead of copying classic-dialog pixels.

## Responsive Behavior and Input

The virtualized grid supports arrows, Home/End, Page, and text search; each icon exposes name, category, mirroring, and source rather than only a Unicode value. Narrow layouts use collapse, overflow, or one column and wide layouts cap content width; 200% text scaling never clips commit/cancel paths.

## Motion and Accessibility

State transitions last at most 200 ms and Reduced Motion switches instantly. High Contrast uses system colors and non-color cues; Automation exposes structure, state, validation, and next action.
