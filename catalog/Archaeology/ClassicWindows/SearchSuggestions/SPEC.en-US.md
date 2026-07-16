# Search Suggestions Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Windows search evolved from Vista/7 instant indexed results into categorized suggestions, recent queries, app/file/settings results, and discovery entries across Windows 8–11.

## Design DNA

Feedback while typing, source grouping, keyboard preview selection, distinction between query and suggestion commit, and explicit no-results feedback.

## Modern Reconstruction

Reconstruct as SearchBoxEx: hosts inject cancellable multi-source Providers and the control merges groups, recents, and discovery entries; it never uploads queries or accesses Windows Search index by default.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.search-box-ex (SearchBoxEx). The exhibit declares no public API, type, or resource key and duplicates no stable control; historical labels explain provenance only.

## States and Failure Modes

Idle, Typing, Debouncing, Loading, Results, Empty, and Error; stale query results are version-discarded and IME composition never commits search. Scale, format, or Provider errors degrade locally without corrupting committed value, selection, or host data.
