# Wizard / Stepper Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Progressive information collection, per-step validation, reversibility before commit, explicit confirmation before irreversible actions, and next paths after completion/failure.

## Modernize

Reconstruct as WizardStepper: hosts declare a step graph, validation, and commit commands; the control provides horizontal/vertical progress, save/resume, branching, and focus management. Appearance uses current Fluent ThemeResources, system typography, and modern focus visuals instead of copying classic-dialog pixels.

## Responsive Behavior and Input

Keyboard Tab remains within the current step, Alt+Left/Back returns without bypassing policy; screen readers announce step number, title, errors, and commit state. Narrow layouts use collapse, overflow, or one column and wide layouts cap content width; 200% text scaling never clips commit/cancel paths.

## Motion and Accessibility

State transitions last at most 200 ms and Reduced Motion switches instantly. High Contrast uses system colors and non-color cues; Automation exposes structure, state, validation, and next action.
