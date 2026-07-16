# Advanced Color Picker Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Coexistence of visual exploration and precise numeric input, synchronized color spaces, recent/custom color memory, optional alpha, and immediate validation.

## Modernize

Reconstruct as ColorPickerEx, adding palettes, history, favorites, an eyedropper Provider, and configurable gamut to the modern ColorPicker; host owns screen-capture permission and color management. Appearance uses current Fluent ThemeResources, system typography, and modern focus visuals instead of copying classic-dialog pixels.

## Responsive Behavior and Input

Channels support keyboard fine adjustment and spectrum has 2D Automation value plus text description; current color is never color-only and includes copyable numeric value. Narrow layouts use collapse, overflow, or one column and wide layouts cap content width; 200% text scaling never clips commit/cancel paths.

## Motion and Accessibility

State transitions last at most 200 ms and Reduced Motion switches instantly. High Contrast uses system colors and non-color cues; Automation exposes structure, state, validation, and next action.
