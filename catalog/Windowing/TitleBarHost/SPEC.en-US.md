# TitleBarHost Specification

## Goal

Provide composable XAML title-bar layout that generates drag/interactive regions and synchronizes with host WindowChrome.

## Host and Window Boundary

Own title-bar visuals and region descriptions only, not AppWindow creation/closure; host forwards regions to WindowChrome and owns system title/lifecycle.

## Candidate Surface and Lock Conditions

Candidate concepts: Icon, Title, Subtitle, LeftContent, CenterContent, RightContent, PreferredHeight, RegionsChanged. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Unloaded --> Measuring --> Ready\nReady --> RegionsDirty --> Measuring\nAny --> HighContrast/Inactive\nAny --> Unloaded
```

## Candidate Template Parts and Visual Tree

Candidate tree: RootGrid, IconPresenter, TextStack, Left/Center/RightPresenter, DragSurface, CaptionInsetSpacer. Interactive content is automatically excluded from drag.

## Behavior and Failure Modes

Publish regions only after stable layout; unload/hidden content removes exclusions immediately; long title ellipsizes while Automation keeps full text.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

