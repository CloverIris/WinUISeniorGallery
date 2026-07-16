# Property Grid Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

The Visual Basic and Visual Studio Properties window has long edited current selection through categorized/alphabetical modes, name-value columns, nested objects, a description pane, and type-specific editors.

## Design DNA

Selection-driven reflective editing, separation of metadata and editors, common properties for multi-selection, inline validation, and host-transaction undo.

## Modern Reconstruction

Reconstruct as an extensible PropertyGrid: hosts supply descriptors, editor factories, and change transactions; the control owns categories, filtering, virtualization, validation, and keyboard navigation.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.property-grid (PropertyGrid). The exhibit declares no public API, type, or resource key and duplicates no stable control; historical labels explain provenance only.

## States and Failure Modes

NoSelection, Single, Multiple, Editing, Validating, ReadOnly, and Error; removing the edited object cancels its transaction and clears the editor. Scale, format, or Provider errors degrade locally without corrupting committed value, selection, or host data.
