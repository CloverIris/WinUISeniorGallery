# Wizard / Stepper Integration

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Dependency Direction

Depend on controls.wizard-stepper and allow only Archaeology/Gallery to point to stable modules. This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.wizard-stepper (WizardStepper).

## Data and Platform

Gallery uses deterministic local fake data. File system, Shell, system fonts, search index, reflection, or async data sources enter only through host Providers; the exhibit never scans the device or mutates data itself.

## Global Contracts

Use Theme, Motion, Input, Accessibility, Navigation, Localization, Resources, and ExternalApis contracts. Async requests carry version/cancellation and late results are ignored after closure.

## Degradation and Copyright

When platform APIs are unavailable, use safe fake data and explain limits. Never extract system-DLL icons, copy Windows screenshots/sounds/fonts, or commit third-party content; license_review stays pending until release review.
