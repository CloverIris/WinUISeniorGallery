# Zune Quickplay Research Specification

## Historical Prototype Structure

Zune HD/software Quickplay composes personal pinned, recent, history, and new entry groups into a scrolling home with oversized category text and content previews. QuickplayHeader, fixed category Sections, preview items/count, pin management, and recent history; it is home information architecture, not one control.

## Historical Interaction States

```text
LoadingSections --> Ready\nReady --> SectionFocused/ItemInvoked\nReady --> Pinning/Reordering --> Ready\nAny --> EmptySection/PartialError
```

## Preserved Design DNA

Start from personal activity rather than media type, pinned beside recent, few high-value entries, and giant headings as navigation landmarks.

## Explicitly Discarded and Modernized

Discard fixed categories/hard-coded sources, opaque history privacy, and touch-only reorder; support disabling history, Provider sections, keyboard reorder, and empty states.

## Modern Owner and API Boundary

HomeScreen owns Section composition/data; exhibit provides a Quickplay layout sample and owns no Pin/History service API. Dependency points only from Archaeology/Gallery to experiences.home-screen; the exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (HomeScreen)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner, demo data, or effect shows a static diagram and reason and never fakes the original product. Before specified: recheck sources, asset license, prototype states, and modern differences.

