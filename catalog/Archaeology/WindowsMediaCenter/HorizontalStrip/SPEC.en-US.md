# Media Center Horizontal Strip Research Specification

## Historical Prototype Structure

Windows Media Center organizes movies, recordings, music, and photos as horizontal strips at TV distance; focus scrolls into a safe zone with slight scale while remote left/right browses. SectionHeader, HorizontalViewport, uniform ItemCards, FocusChrome, edge peek, and optional DetailsPresenter form one category rail.

## Historical Interaction States

```text
Empty --> Loading --> Ready\nReady --> FocusMoving --> Settling --> Ready\nReady --> ItemInvoked\nAny --> PartialError/Unloaded
```

## Preserved Design DNA

10-foot legibility, one-axis focus, edge peeks, focused-item emphasis without relayout, and vertical stacking of category rails.

## Explicitly Discarded and Modernized

Discard remote-only input, fixed TV resolution, scale that obscures neighbors, and eager library load; add touch/mouse/keyboard, virtualization, and safe zone.

## Modern Owner and API Boundary

ContentRail owns stable list/virtualization/focus API; exhibit supplies a Media Center preset and research only. Dependency points only from Archaeology/Gallery to controls.content-rail; exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
ExhibitPage
|- Header (origin, era, proposed/pending)
|- PrototypeStructureDiagram (no original assets)
|- DesignDnaAndTradeoffs
|- ModernDemo (ContentRail)
|- InputAccessibilityMatrix
`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner/effect/data shows static structure and reason, never faking the product. Before specified, review sources, asset license, states, and modern differences.

