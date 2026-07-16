# Pivot Research Specification

## Historical Prototype Structure

Windows Phone 7/8 lightweight top navigation: oversized horizontal text headers extend beyond the viewport and swipe in sync with adjacent pages. Visual structure is HeaderStrip, SelectedHeader, peeking neighbor headers, and one ContentViewport; it does not require every page realized.

## Historical Interaction States

```text
Idle --> DragTracking --> Settling --> Selected\nSelected --> HeaderInvoked --> Settling\nAny --> Disabled/Unloaded
```

## Preserved Design DNA

Lightweight navigation, discoverable neighbors, content-sized headers, and one motion progress shared by header and content.

## Explicitly Discarded and Modernized

Discard gesture-only navigation, default infinite loop, clipped giant headers, and eager pages; add explicit click, focus, overflow, and virtualization.

## Modern Owner and API Boundary

PivotView owns stable implementation; the exhibit explains WP structure and configures that modern control to compare touch/keyboard behavior. Dependency points only from Archaeology/Gallery to controls.pivot-view; the exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (PivotView)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner, demo data, or effect shows a static diagram and reason and never fakes the original product. Before specified: recheck sources, asset license, prototype states, and modern differences.

