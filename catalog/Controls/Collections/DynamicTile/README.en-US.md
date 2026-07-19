# DynamicTile

Specification work item for DynamicTile. Business-logic implementation is now active; the host still owns the visual template and final acceptance.

## Status

in-progress / lab / P1. Logic is available for host integration; the item is not yet review-ready.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Controls/Collections/DynamicTile

## Current implementation surface

`DynamicTile` exposes a stable `Id`, `Frames`, `Size`, `Transition`, automatic/manual rotation, host visibility/window-active pause, and a cancellable host `RefreshProvider`.
`DynamicTileBoard` coordinates deterministic ordering, edit-mode reordering, pause broadcasts, and parallel refresh; it never creates windows, registers system tiles, or persists content.

This is an app-local dynamic card, not a system tile. The host must still lock the visual update queue before Ready; the default policy retains at most the newest three faces.
