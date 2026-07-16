# Zune Album Art Wall Research Specification

## Historical Prototype Structure

Zune software used dense album-art walls in browse/playback backgrounds, visualizing a library as a continuous mosaic with current album/artist emphasized. Regular/semi-regular CoverGrid, cropped covers, selection emphasis, and optional dim background; text stays in a separate foreground.

## Historical Interaction States

```text
Empty --> LoadingThumbnails --> Ready\nReady --> Focus/SelectionChanged --> Ready\nAny --> PartialImages/StaticFallback
```

## Preserved Design DNA

Content becomes brand atmosphere, repeated grid creates rhythm, and current item stands out through scale/luminance rather than heavy borders.

## Explicitly Discarded and Modernized

Discard eager full-library load, unstable random layout, low-contrast text, and unlicensed imagery; use virtualization, stable IDs, scrim, placeholders, licensed assets.

## Modern Owner and API Boundary

AdaptiveGrid owns grid/virtualization; exhibit composes it to show Zune DNA and owns no cover-loading/selection API. Dependency points only from Archaeology/Gallery to controls.adaptive-grid; the exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (AdaptiveGrid)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner, demo data, or effect shows a static diagram and reason and never fakes the original product. Before specified: recheck sources, asset license, prototype states, and modern differences.

