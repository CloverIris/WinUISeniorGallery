# Tree Data Grid Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Windows management, Explorer, and developer tools have long combined tree hierarchy with report-view columns for processes, projects, objects, and resources; this is a cross-product pattern, not a single-origin control.

## Design DNA

Hierarchy expansion in the first column, aligned comparison columns, full-row selection, column sort/resize, deep virtualization, and keyboard tree navigation.

## Modern Reconstruction

Reconstruct as a data-source-driven TreeDataGrid: hosts provide stable node IDs, asynchronous children, and columns; the control owns viewport flattening, virtualization, selection, sort requests, and expansion state.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.tree-data-grid (TreeDataGrid). The exhibit declares no public API, type, or resource key and duplicates no stable control; historical labels explain provenance only.

## States and Failure Modes

LoadingRoot, Ready, ExpandingNode, Partial, Empty, and Error; one node load failure affects only that branch and retry does not collapse siblings. Scale, format, or Provider errors degrade locally without corrupting committed value, selection, or host data.
