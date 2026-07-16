# Live Tile Research Specification

## Historical Prototype Structure

Windows Phone Start Live Tiles use small/medium/wide sizes for app identity and dynamic summaries, with flip/cycle, icon, and count before opening the app. TileSurface contains brand, Primary/SecondaryContent, Badge, and size variant; the system schedules updates and Start owns layout.

## Historical Interaction States

```text
Static --> UpdateQueued --> Transitioning --> Live\nLive --> Stale/Paused --> Live\nAny --> Disabled
```

## Preserved Design DNA

Glanceable information, fixed size variants, brief dynamic rotation, unified identity/status, and user-owned layout.

## Explicitly Discarded and Modernized

Do not emulate OS Start, background Tile APIs, endless flips, or uncontrolled notification; modernize as an in-app dashboard card with pause.

## Modern Owner and API Boundary

DynamicTile owns stable size/content/motion API; exhibit maps historical sizes/states with deterministic local data. Dependency points only from Archaeology/Gallery to controls.dynamic-tile; the exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (DynamicTile)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner, demo data, or effect shows a static diagram and reason and never fakes the original product. Before specified: recheck sources, asset license, prototype states, and modern differences.

