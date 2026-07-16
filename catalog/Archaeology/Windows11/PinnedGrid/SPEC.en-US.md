# Start Menu Pinned Grid Research Specification

## Historical Prototype Structure

Windows 11 Start uses a centered adaptive app-icon grid for Pinned items, with paging, drag reorder, and later folders, separated from Recommended. StartSurface contains PinnedHeader, fixed-column Grid, PageIndicator, FolderTile/FolderFlyout, AllApps entry, and Recommended section.

## Historical States and Focus

```text
Ready --> Reordering --> Ready\nReady --> FolderOpen --> Ready\nReady --> Paging --> Ready\nAny --> Empty/PartialIconError
```

Arrows/gamepad navigate grid, Enter/A invokes, menu manages, and keyboard alternatives move/create folder; icons expose text names rather than graphics only.

## Preserved Design DNA

Pinned frequent entry, uniform icon rhythm, user reorder, folders for density, and paging that preserves predictable positions.

## Modernization and Discarded Boundary

Discard emulating Start, launching system apps, reading real pins, Recommended/ads, system folder drops, and Windows icon assets; extract an adaptive pinned grid only.

## Modern Feature Owner

Owner registry: `controls.adaptive-grid` (`AdaptiveGrid`).

AdaptiveGrid owns layout/virtualization; host experience owns folders, reorder transaction, and launch commands, and the exhibit owns no such API. Archaeology dependencies point only Gallery/Archaeology to owner; stable modules never reference exhibits.

## Gallery Research Tree

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (AdaptiveGrid)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## Failure and Promotion Locks

Missing owner/capability/data retains static research and never fakes system behavior. Before specified, lock sources, owner, asset license, differences, Automation semantics, and platform exclusion zone.
