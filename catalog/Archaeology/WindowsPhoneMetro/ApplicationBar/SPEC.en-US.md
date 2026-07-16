# Application Bar Research Specification

## Historical Prototype Structure

Windows Phone 7/8 Application Bar is bottom-fixed with up to four circular primary commands; ellipsis expands labels and secondary menu upward. PrimaryIconRow, EllipsisButton, PrimaryLabels, SecondaryMenu, and translucent Backplate; primary icons remain when collapsed.

## Historical Interaction States

```text
Collapsed --> Expanding --> Expanded\nExpanded --> CommandInvoked/Collapsing --> Collapsed\nAny --> Disabled/Hidden
```

## Preserved Design DNA

Thumb-reachable bottom actions, four-primary limit, progressive disclosure, text labels on expand, and retained content context.

## Explicitly Discarded and Modernized

Discard ellipsis guesswork, fixed phone height, icons without ToolTips, and keyboard-inaccessible secondary commands; add explicit names, overflow, and focus.

## Modern Owner and API Boundary

ExpandableCommandBar owns command model/expansion; exhibit supplies a WP arrangement preset and historical explanation. Dependency points only from Archaeology/Gallery to controls.expandable-command-bar; the exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (ExpandableCommandBar)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner, demo data, or effect shows a static diagram and reason and never fakes the original product. Before specified: recheck sources, asset license, prototype states, and modern differences.

