# Media Center Grid Research Specification

## Historical Prototype Structure

Windows Media Center poster/library grids are remote-first; arrows move in 2D and the focused card is emphasized with title/details in the same view. GridViewport, virtualized poster rows/columns, FocusChrome, Page/SectionHeader, and DetailsOverlay; selection and focus may differ.

## Historical Interaction States

```text
Empty --> Loading --> Ready\nReady --> FocusMoving/DetailsVisible --> Ready\nReady --> ItemInvoked\nAny --> PartialError/Unloaded
```

## Preserved Design DNA

Stable 2D space, 10-foot focus, poster-first content, focus-driven details, and clear Back path.

## Explicitly Discarded and Modernized

Discard fixed columns, low-resolution bitmaps, remote-only input, scale relayout, and eager realization; use responsive columns, licensed images, render transform, and virtualization.

## Modern Owner and API Boundary

MediaCenterGrid owns stable grid behavior; exhibit maps original structure/10-foot preset and declares no grid API. Dependency points only from Archaeology/Gallery to media.media-center-grid; exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
ExhibitPage
|- Header (origin, era, proposed/pending)
|- PrototypeStructureDiagram (no original assets)
|- DesignDnaAndTradeoffs
|- ModernDemo (MediaCenterGrid)
|- InputAccessibilityMatrix
`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner/effect/data shows static structure and reason, never faking the product. Before specified, review sources, asset license, states, and modern differences.

