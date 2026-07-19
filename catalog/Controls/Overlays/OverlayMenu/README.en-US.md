# OverlayMenu

Specification work item for OverlayMenu.

## Status

in-progress / lab / P2. Hierarchical navigation, modal scrim, and Escape/back behavior are implemented locally.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Controls/Overlays/OverlayMenu

## Implementation readiness
The hierarchical menu defaults to Modal and Right placement. Escape returns to the parent before closing the root; a non-modal menu does not auto-close after a leaf invoke. The host owns item data and actions.
