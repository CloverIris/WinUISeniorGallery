# DragOverlay

`DragOverlay` is a host-driven visual feedback layer for drag operations. It renders preview, caption, position, and allowed-state feedback without owning transfer data, hit testing, window creation, or cross-window coordinate conversion.

## Status

in-progress / lab / P2.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Controls/Overlays/DragOverlay

Implementation: `src/WinUI3.Senior.Controls/Overlays/DragOverlay`

## Implementation readiness
The current implementation provides `Show`, `Update`, `BeginDrop`, and `Hide`, with `Hidden`, `Allowed`, `Forbidden`, and `Dropping` states. `DropRequested` must be explicitly accepted by the host before the preview is dismissed; an empty operation set is forbidden. The control is non-hit-testable by default and clears source/preview references when hidden. Cross-window coordinates and Escape cancellation remain host-owned; Gallery integration and automated verification are follow-up work.
