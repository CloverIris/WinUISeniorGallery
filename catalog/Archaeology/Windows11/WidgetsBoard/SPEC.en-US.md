# Widgets Board Research Specification

## Historical Prototype Structure

Windows 11 Widgets Board opens from the taskbar as a side surface combining resizable widget cards, personalized feed, search/account entry, and scrolling content. BoardChrome, WidgetGrid, Small/Medium/Large cards, drag reorder, More menu, refresh state, and Feed section form the layered surface.

## Historical States and Focus

```text
Closed --> Opening --> LoadingBoard --> Ready\nReady --> Reordering/Resizing/Refreshing --> Ready\nReady --> WidgetError/FeedPartial\nAny --> Closing --> Closed
```

Keyboard/gamepad uses 2D card navigation, menu opens actions, and drag has keyboard alternative; close restores entry focus and refresh never steals focus.

## Preserved Design DNA

Glanceable dynamic cards, standard size tiers, user pin/reorder, local refresh/error isolation, and a side entry that does not replace the primary task.

## Modernization and Discarded Boundary

Discard system taskbar entry, personalized news feed, account tracking, unbounded background refresh, web widgets, and system-board appearance; modernize as app-owned dashboard.

## Modern Feature Owner

Owner registry: `experiences.widgets-board` (`WidgetsBoard`).

WidgetsBoard owns modern card layout/lifecycle; exhibit configures local anonymous WidgetProviders and Windows 11-inspired information architecture only. Archaeology dependencies point only Gallery/Archaeology to owner; stable modules never reference exhibits.

## Gallery Research Tree

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (WidgetsBoard)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## Failure and Promotion Locks

Missing owner/capability/data retains static research and never fakes system behavior. Before specified, lock sources, owner, asset license, differences, Automation semantics, and platform exclusion zone.
