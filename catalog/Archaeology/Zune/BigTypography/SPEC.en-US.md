# Zune Big Typography Research Specification

## Historical Prototype Structure

Zune and Metro use oversized, light-weight, often lowercase Segoe titles as spatial/brand elements; titles may scroll partly offscreen, replacing conventional chrome. BigTitle, optional Eyebrow/Subtitle, content baseline, and CompactTitle after scroll collapse/pin; typography itself defines hierarchy.

## Historical Interaction States

```text
Expanded --> Collapsing(progress) --> Compact\nCompact --> Expanding --> Expanded\nAny --> Wrapped/ClampedFallback
```

## Preserved Design DNA

Bold scale, whitespace, light weight, content-first hierarchy, and continuous title transformation rather than sudden replacement.

## Explicitly Discarded and Modernized

Discard fixed 72pt, English lowercase assumptions, clipping, poor CJK/RTL adaptation, and low-contrast thin weight; use language-aware scale/weight and text scaling.

## Modern Owner and API Boundary

BigTitle owns typography/scroll state; exhibit supplies a Zune preset and rationale without new font/title API. Dependency points only from Archaeology/Gallery to controls.big-title; the exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (BigTitle)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner, demo data, or effect shows a static diagram and reason and never fakes the original product. Before specified: recheck sources, asset license, prototype states, and modern differences.

