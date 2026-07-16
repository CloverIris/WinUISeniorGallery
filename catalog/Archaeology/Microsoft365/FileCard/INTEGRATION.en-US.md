# File Card Integration

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Dependency Direction

Depend on experiences.file-card; only Archaeology/Gallery points to stable modules and stable modules never reference the exhibit. This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by experiences.file-card (FileCard).

## Data and Services

Gallery uses anonymous deterministic fake data. Microsoft Graph, Azure, file-system, read-aloud, or collaboration services are host-injected Providers owning authentication, consent, cancellation, and rate limits.

## Global Contracts

Use Theme, Motion, Input, Accessibility, Navigation, Localization, Resources, and ExternalApis contracts. Every async result binds to a request version and late responses are ignored after page closure.

## Degradation and Copyright

Offline or limited permission preserves local structure and explanation. Never extract Office/Microsoft 365 icons, screenshots, sounds, templates, or fonts; demo assets are owned or license-compatible and license_review remains pending until release review.
