# Font Picker Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Font names previewed in their own face, layered family/face/size selection, system-font enumeration, missing-font fallback, keyboard type-ahead, and recents.

## Modernize

Reconstruct as a virtualized FontPicker: the host supplies a font catalog and license metadata while the control owns search, preview, favorites/recents, and fallback explanation; it neither bundles nor uploads system fonts. Appearance uses current Fluent ThemeResources, system typography, and modern focus visuals instead of copying classic-dialog pixels.

## Responsive Behavior and Input

Search supports IME, arrows browse, and host policy distinguishes Enter preview/commit; screen readers announce font name and status rather than attempting visual-shape description. Narrow layouts use collapse, overflow, or one column and wide layouts cap content width; 200% text scaling never clips commit/cancel paths.

## Motion and Accessibility

State transitions last at most 200 ms and Reduced Motion switches instantly. High Contrast uses system colors and non-color cues; Automation exposes structure, state, validation, and next action.
