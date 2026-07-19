# MixviewExperience Specification

## Goal

Provide a radial related-content graph over host-owned nodes. The control owns layout, selection, and accessibility announcements; it does not own navigation, recommendations, network access, or windows.

## Non-goals

No remote recommendations, image/media loading, persisted layout, window creation, or cross-page navigation.

## Public API

`MixNode(Id, Title, Kind, RelatedIds, Content, Tag)`; `MixviewExperience.Nodes`, `RootNodeId`, `SelectedNodeId`, `MaxVisibleNodes` (1–64, default 12), `IsReducedMotion`, and `IsOpen`. `SetNodes` validates and de-duplicates IDs; `Open`, `Close`, and `SelectNode` expose observable results. `NodeSelected` carries `IsUserInitiated`; `Closed` only signals host-owned dismissal.

## State model

An empty collection is closed/empty. Nodes with `IsOpen=false` are closed; an opened graph is open/selected. Unknown IDs return false without changing selection. `SelectedNode` prefers `SelectedNodeId` and otherwise uses the root.

## Template parts and visual tree

Required parts: `PART_Surface` (Canvas) and `PART_LiveRegion` (TextBlock). A missing surface disables rendering without preventing construction; a missing live region disables announcements only. Node buttons are created with themed visuals and Automation names.

## Behavior and failure modes

The selected root is centered. Related nodes use deterministic radial positions and are truncated at `MaxVisibleNodes`; when no related IDs resolve, other nodes are used as a fallback. Escape closes. Empty collections and unknown IDs return false without throwing. Reduced Motion changes only future visual transitions, not layout or selection.

## Failure and degradation

Invalid node data causes `SetNodes` to throw `ArgumentException` while retaining the previous collection. Optional template regions degrade independently. Navigation and content lifetime remain host-owned.

## Scenario, data, and visual tree

The model is Node → RelatedIds; the tree is `Border → Canvas(PART_Surface) + LiveRegion`. Identical node order produces identical layout for Gallery and automation verification.
