# Font Picker Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Win32 ChooseFont and Office font dropdowns have long offered family, style, size, script, and live preview, later adding recents, theme fonts, and search.

## Design DNA

Font names previewed in their own face, layered family/face/size selection, system-font enumeration, missing-font fallback, keyboard type-ahead, and recents.

## Modern Reconstruction

Reconstruct as a virtualized FontPicker: the host supplies a font catalog and license metadata while the control owns search, preview, favorites/recents, and fallback explanation; it neither bundles nor uploads system fonts.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.font-picker (FontPicker). The exhibit declares no public API, type, or resource key and duplicates no stable control; historical labels explain provenance only.

## States and Failure Modes

Enumerating, Ready, Filtering, Previewing, Missing, and Restricted; load failure falls back to system UI font while preserving the requested name with warning. Scale, format, or Provider errors degrade locally without corrupting committed value, selection, or host data.
