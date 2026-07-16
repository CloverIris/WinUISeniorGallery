# Advanced Color Picker Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

The Win32 ChooseColor common dialog and later UWP/WinUI ColorPicker provide basic/custom palettes, precise RGB/HSV/HEX input, and spectrum selection.

## Design DNA

Coexistence of visual exploration and precise numeric input, synchronized color spaces, recent/custom color memory, optional alpha, and immediate validation.

## Modern Reconstruction

Reconstruct as ColorPickerEx, adding palettes, history, favorites, an eyedropper Provider, and configurable gamut to the modern ColorPicker; host owns screen-capture permission and color management.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.color-picker-ex (ColorPickerEx). The exhibit declares no public API, type, or resource key and duplicates no stable control; historical labels explain provenance only.

## States and Failure Modes

Spectrum, Numeric, Palette, Eyedropper, Invalid, and Unavailable; all inputs synchronize through one canonical color and invalid in-progress text never corrupts the committed value. Scale, format, or Provider errors degrade locally without corrupting committed value, selection, or host data.
