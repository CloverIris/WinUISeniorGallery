# Snap Layouts Research Specification

## Historical Prototype Structure

Windows 11's 2021 release surfaces Snap Layouts from maximize-button hover or Win+Z, presenting zone diagrams for window placement and grouping snapped windows. The system entry contains MaximizeButton flyout, LayoutTemplates, zone hit regions, current-window preview, and subsequent group filling; it belongs to Shell non-client UI.

## Historical States and Focus

```text
Hidden --> Triggered(hover/Win+Z) --> LayoutChoosing\nLayoutChoosing --> ZoneFocused --> Committing --> Hidden\nCommitting --> GroupFilling --> Hidden\nAny --> Canceled/Unavailable
```

Research page demonstrates pointer hover/click, arrows, and Enter but never registers Win+Z; Escape cancels preview and restores drag-source focus.

## Preserved Design DNA

Layout choice near the action point, graphical rather than numeric ratios, keyboard/pointer parity, preview before commit, and discoverable common multitasking arrangements.

## Modernization and Discarded Boundary

Modern Demo explicitly discards imitating maximize flyout, moving real OS windows, creating Snap Groups, covering system zones, or using Windows trademark graphics; it extracts in-app Dock preview only.

## Modern Feature Owner

Owner registry: `windowing.dock-layout-preview` (`DockLayoutPreview`).

DockLayoutPreview is the sole modern owner and manages app-internal panel drop only; Windows/Shell continues to own system Snap. Archaeology dependencies point only Gallery/Archaeology to owner; stable modules never reference exhibits.

## Gallery Research Tree

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (DockLayoutPreview)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## Failure and Promotion Locks

Missing owner/capability/data retains static research and never fakes system behavior. Before specified, lock sources, owner, asset license, differences, Automation semantics, and platform exclusion zone.
