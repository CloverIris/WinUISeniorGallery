# WindowChrome Specification

## Goal

Coordinate custom title bar, system caption regions, backdrop, and non-client hit testing for one Window.

## Host and Window Boundary

One instance binds one host Window/AppWindow; it creates no window, owns no page content, and changes no navigation. Host retains it, closes it, and presents errors.

## Candidate Surface and Lock Conditions

Candidate concepts: Window, TitleBarContent, BackdropKind, ExtendIntoTitleBar, InteractiveRegions, Attach/Detach, ChromeState. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Detached --> Attaching --> Active\nActive --> Reconfiguring --> Active\nAny --> Failed --> Detached\nActive/Failed --> Closing --> Closed
```

## Candidate Template Parts and Visual Tree

Candidate tree: ChromeRoot, TitleBarHost, ContentPresenter, BackdropLayer; system draws CaptionButtons by default and names freeze before ready.

## Behavior and Failure Modes

Attach is idempotent and rebinding first detaches; DPI/size/theme recompute drag and exclusions in one batch; every call after Closed fails observably.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

