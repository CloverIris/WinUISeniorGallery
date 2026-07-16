# Panorama Background Research Specification

## Historical Prototype Structure

Windows Phone 7 Panorama uses an ultrawide image across content panes; foreground travels farther than background to create depth and continuity. Clipped BackgroundLayer, optional Tint/Scrim, PanoramaTitle, and independent ForegroundStrip; background never participates in hit testing.

## Historical Interaction States

```text
Idle --> Panning(progress) --> Settling\nTheme/AssetChanged --> RecomputingCrop --> Idle\nAny --> StaticFallback
```

## Preserved Design DNA

Shared background connects Sections, controlled parallax expresses depth, and content motion drives background rather than autonomous animation.

## Explicitly Discarded and Modernized

Discard device-specific ultrawide crop, text directly on noisy imagery, excessive motion, and blocking preload; support focal crop, scrim, and static fallback.

## Modern Owner and API Boundary

HubPanorama owns background/parallax configuration; exhibit explains historical math with owned abstract art and provides no API. Dependency points only from Archaeology/Gallery to experiences.hub-panorama; the exhibit declares no type, resource key, service, or platform capability.

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

