# Hub Research Specification

## Historical Prototype Structure

Windows Phone 8.1 Hub places heterogeneous Sections on a horizontal panoramic canvas, with per-section templates and layered title/background motion. HubHeader, horizontal SectionStrip, variable-width Sections, SectionHeader, background layer, and semantic jump entry form a continuous surface.

## Historical Interaction States

```text
Loading --> Ready --> Panning --> Settling --> Ready\nReady --> SectionInvoked/NestedScroll\nAny --> PartialError/Unloaded
```

## Preserved Design DNA

Heterogeneous content, horizontal exploration, section headings, global background continuity, and editorial emphasis on the first viewport.

## Explicitly Discarded and Modernized

Discard eager infinite width, uncontrolled background contrast, nested horizontal conflicts, and gesture-only entry; use section virtualization, scrims, and explicit jump.

## Modern Owner and API Boundary

HubPanorama owns implementation/layout; the exhibit provides diagrams, licensed fake content, and adjustable parallax without owning Section API. Dependency points only from Archaeology/Gallery to experiences.hub-panorama; the exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (HubPanorama)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner, demo data, or effect shows a static diagram and reason and never fakes the original product. Before specified: recheck sources, asset license, prototype states, and modern differences.

