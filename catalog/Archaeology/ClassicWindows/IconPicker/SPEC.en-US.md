# Icon Picker Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

The Windows Shell Change Icon dialog enumerates icons from files/resources, while modern Windows adds searchable symbol sets through Segoe MDL2 and Segoe Fluent Icons.

## Design DNA

Grid browsing of large sets, explicit current selection, switchable source path, separation of preview and resource index, and keyboard type-ahead.

## Modern Reconstruction

Reconstruct as a safe IconPicker supporting app-registered Symbol/IconSource catalogs, search, categories, favorites, and RTL metadata; it does not scan arbitrary DLL/EXE files or extract system resources by default.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.icon-picker (IconPicker). The exhibit declares no public API, type, or resource key and duplicates no stable control; historical labels explain provenance only.

## States and Failure Modes

LoadingCatalog, Ready, Filtering, Empty, SourceUnavailable, and Selected; revoking a source clears preview while retaining an explanatory selection descriptor. Scale, format, or Provider errors degrade locally without corrupting committed value, selection, or host data.
