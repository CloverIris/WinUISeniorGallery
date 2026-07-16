# Zune Mixview Research Specification

## Historical Prototype Structure

Zune 3.0-era Mixview centers an artist/album and expands related recommendations, tracks, and relationships as varied tiles; invoking a node recenters the network. CenterNode, radial RelatedNodes, relationship-weighted sizes, connection semantics, and detail layer; not a generic physics graph.

## Historical Interaction States

```text
Empty --> LoadingRelations --> Ready\nReady --> NodeInvoked --> Reflowing --> Ready\nReady --> Panning/Zooming\nAny --> PartialError
```

## Preserved Design DNA

Visible relationships, explicit center context, size encoding importance, and continued exploration through recentering.

## Explicitly Discarded and Modernized

Discard unexplained randomness, endless motion, color-only relationships, eager nodes, and keyboard-inaccessible graph; use deterministic layout, legend, paging, and list alternative.

## Modern Owner and API Boundary

MixviewExperience owns relation model/layout; exhibit defines historical preset, fake data, and comparison only, with no graph API. Dependency points only from Archaeology/Gallery to experiences.mixview; the exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (MixviewExperience)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner, demo data, or effect shows a static diagram and reason and never fakes the original product. Before specified: recheck sources, asset license, prototype states, and modern differences.

