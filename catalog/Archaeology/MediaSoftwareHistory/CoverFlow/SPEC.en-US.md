# Cover Flow Research Specification

## Historical Prototype Structure

Cover Flow is not a Zune invention. Apple listed Cover Flow as a new iTunes 7 feature on 2006-09-12: a frontal center cover with perspective-angled neighbors changed by wheel, drag, or keys. CenterCover, left/right PerspectiveNeighbors, Reflection/Shadow, horizontal index, and current metadata form a 3D-style browser; center is the primary invoke target.

## Historical Interaction States

```text
Idle --> Dragging/Wheeling --> InertialSettling --> Centered\nCentered --> ItemInvoked\nAny --> FlatFallback/Unloaded
```

## Preserved Design DNA

Strong centered hierarchy, neighbors express sequence, continuous scroll/perspective, and a physical, glanceable cover browser.

## Explicitly Discarded and Modernized

Discard mandatory reflection, excessive 3D/motion, eager textures, LTR-only layout, and Zune attribution; use virtualization, optional effects, flat Reduced Motion, and RTL.

## Modern Owner and API Boundary

CarouselView owns stable carousel/virtualization/input API; exhibit supplies a CoverFlow transition preset and media-software-history research without copying Apple UI. Dependency points only from Archaeology/Gallery to controls.carousel-view; exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
ExhibitPage
|- Header (origin, era, proposed/pending)
|- PrototypeStructureDiagram (no original assets)
|- DesignDnaAndTradeoffs
|- ModernDemo (CarouselView)
|- InputAccessibilityMatrix
`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner/effect/data shows static structure and reason, never faking the product. Before specified, review sources, asset license, states, and modern differences.

