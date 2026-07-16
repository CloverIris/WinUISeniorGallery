# Editor Canvas Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Word's paginated document surface and PowerPoint's slide-editing surface have long combined page/canvas, rulers, zoom, guides, object selection, and thumbnail navigation.

## Design DNA

Separation of content surface and tool chrome, stable coordinate space, continuous zoom, alignment guides, page/slide navigation, and selection-driven contextual tools.

## Modern Reconstruction

Reconstruct as a generic editor-canvas shell composing zoom/pan, page thumbnails, rulers, and guides; document model, graphics rendering, undo, and collaboration belong to the host or future Canvas module.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by experiences.editor-canvas (EditorCanvasExperience). The exhibit declares no public API and duplicates no modern component. Historical names are research titles only and the demo is labelled Modern Reconstruction.

## States and Failure Modes

Browse, Select, Pan, Edit, and PresentationPreview; mode changes preserve zoom center and selection, and invalid object selection clears atomically. Missing data, service, or dependency uses local, explanatory degradation and never presents fake data as a real account result.
