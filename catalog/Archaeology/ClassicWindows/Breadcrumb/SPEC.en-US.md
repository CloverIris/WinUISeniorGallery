# Editable Breadcrumb Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Windows Vista Explorer split the address path into clickable hierarchy segments while retaining editable text paths, child drop-downs, and navigation history.

## Design DNA

Persistent current location, direct ancestor access, collapse of early levels under constrained width, and coexistence of visual navigation with exact text entry.

## Modern Reconstruction

Reconstruct as BreadcrumbBarEx, adding edit mode, history/autocomplete, hierarchy menus, and optional drag/drop to the base BreadcrumbBar; path parsing and navigation belong to a host Provider.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.breadcrumb-bar-ex (BreadcrumbBarEx). The exhibit declares no public API, type, or resource key and duplicates no stable control; historical labels explain provenance only.

## States and Failure Modes

Display, Editing, SuggestionsOpen, Navigating, and Invalid; Escape cancels editing and restores the path, while failed navigation preserves input with retryable error. Scale, format, or Provider errors degrade locally without corrupting committed value, selection, or host data.
