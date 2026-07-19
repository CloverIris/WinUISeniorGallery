# EditorCanvas Specification

## Goal

Provide a host-neutral editor-canvas entry point. Core owns immutable document snapshots, thread-safe mutations, viewport math, and pressure-stroke aggregation; the WinUI control owns template, hit testing, input capture, and visual object rendering. File parsing, serialization, and collaboration are outside this phase.

## Non-goals

No file import, persistent page thumbnails, ruler/guide persistence, native C++ renderer, network collaboration, or cloud sync in this phase.

## Public API

Core: `CanvasPoint`, `CanvasSize`, `CanvasRect`, `CanvasTransform`, `CanvasObject`, `CanvasDocumentSnapshot`, `CanvasDocumentController`, `CanvasViewportController`, `CanvasPressureSample`, `CanvasStroke`, and `CanvasInkSession`. WinUI: `EditorCanvas` and `EditorCanvasTool`. The host supplies `Document` and `InkSession`; the control does not create a global document or native pen device.

## State model

The document is empty or host-provided. Tools are `Select`, `Pan`, `Rectangle`, `Text`, and `Ink`; middle mouse/Pan pans, the wheel zooms around the pointer, arrow keys move selection, Delete removes unlocked selection, and Escape clears selection. Rectangle/Text insert a local object on drag release. Ink aggregates `CanvasPressureSample` values into a `CanvasStroke`, committing once on release and producing no object on cancel. Selection and viewport changes are observable and Core `Revision` is monotonic.

## Template parts and visual tree

`PART_Surface` is required; `PART_Grid` and `PART_SelectionAdorner` are optional. If Surface is missing, document operations remain available but rendering is disabled; optional parts only disable their visual layer. Objects are rendered as Canvas children with Automation names and do not require a payload type.

## Behavior and failure modes

An empty document shows an empty surface; invalid geometry or duplicate IDs are normalized/rejected by Core; hosts should detach `Document` on unload and the control never owns a window or file handle. Read-only mode rejects insert/move/delete while allowing zoom, pan, and observation.

## Open Decisions

Open Decisions: page/layer persistence format, retained-mode rendering budget for 100k objects, the C++ Canvas.Native seam, and the pressure-input ABI.

## Scenario, data, and visual tree
Document→Page/Layer→Element; tree `CanvasWinUI→Rulers/Guides→ThumbnailRail`; Loading/Ready/Panning/Editing/Saving/Error. Scenario contract only, no Canvas API.
