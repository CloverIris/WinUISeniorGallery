# People Card Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Outlook and Microsoft 365 profile cards gather avatar, name, role, presence, contact methods, recent collaboration, and quick actions into hover/click flyouts.

## Design DNA

Identity context without navigation, progressive disclosure, unified identity/presence, actions adjacent to the person, and section-level degradation under limited permission.

## Modern Reconstruction

Reconstruct as a data-source-independent PeopleCard: the host supplies a minimal person model and on-demand section Providers; local data displays by default and the card never calls Microsoft Graph itself.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by experiences.people-card (PeopleCard). The exhibit declares no public API and duplicates no modern component. Historical names are research titles only and the demo is labelled Modern Reconstruction.

## States and Failure Modes

Compact, Expanded, LoadingSection, Partial, and Unavailable; sections load, fail, and retry independently without blocking identity and primary actions. Missing data, service, or dependency uses local, explanatory degradation and never presents fake data as a real account result.
