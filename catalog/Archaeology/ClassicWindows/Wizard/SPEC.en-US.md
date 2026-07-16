# Wizard / Stepper Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Windows setup, configuration, and hardware flows have long used multi-page wizards to divide complex work into choice, input, confirmation, execution, and completion with Back/Next/Cancel.

## Design DNA

Progressive information collection, per-step validation, reversibility before commit, explicit confirmation before irreversible actions, and next paths after completion/failure.

## Modern Reconstruction

Reconstruct as WizardStepper: hosts declare a step graph, validation, and commit commands; the control provides horizontal/vertical progress, save/resume, branching, and focus management.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.wizard-stepper (WizardStepper). The exhibit declares no public API, type, or resource key and duplicates no stable control; historical labels explain provenance only.

## States and Failure Modes

Editing, Validating, Blocked, Committing, Completed, Failed, and Canceled; commit prevents duplicate execution and host declares cancellability. Scale, format, or Provider errors degrade locally without corrupting committed value, selection, or host data.
