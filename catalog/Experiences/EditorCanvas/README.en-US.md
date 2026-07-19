# EditorCanvas

Specification work item for EditorCanvas.

## Status

in-progress / lab / P2. The Core document state and WinUI interaction shell are in local implementation; file/native rendering remains future work.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Experiences/EditorCanvas; implementation paths are explicit in feature.json.

## Scenario readiness
Demonstrates Core snapshots, insert/move/delete, selection, undo/redo, zoom and pan, plus a host-supplied pressure stroke session. It does not read files, create windows, persist collaboration, or serialize documents. Ready still requires the 100k-object rendering budget and page/layer persistence boundary.
