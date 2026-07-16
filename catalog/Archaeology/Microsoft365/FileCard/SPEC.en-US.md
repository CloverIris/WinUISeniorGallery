# File Card Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

OneDrive, SharePoint, and Microsoft 365 use file cards/detail flyouts for preview, name, type, author, modified time, sharing state, and quick open/share actions.

## Design DNA

Combine file identity, preview, metadata, and permission state in a compact surface; load progressively, keep actions permission-aware, and provide consistent unknown-type fallback.

## Modern Reconstruction

Reconstruct as a Provider-independent FileCard: the host supplies metadata, preview stream, and commands; the card never reads disk, OneDrive, or SharePoint itself and performs no destructive action.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by experiences.file-card (FileCard). The exhibit declares no public API and duplicates no modern component. Historical names are research titles only and the demo is labelled Modern Reconstruction.

## States and Failure Modes

MetadataOnly, PreviewLoading, Ready, Partial, AccessDenied, and Missing; preview failure never hides the name, and permission changes immediately revoke invalid actions. Missing data, service, or dependency uses local, explanatory degradation and never presents fake data as a real account result.
