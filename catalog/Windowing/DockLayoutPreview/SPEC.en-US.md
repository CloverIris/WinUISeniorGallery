# DockLayoutPreview Specification

## Goal

Show in-app dock targets and magnetic preview during content drag without copying Windows system Snap Layouts.

## Host and Window Boundary

Layout app-internal panels only; move no OS windows, call no system Snap, and cover no caption hover menu. Host owns dragged object and final layout transaction.

## Candidate Surface and Lock Conditions

Candidate concepts: LayoutModel, DraggedItem, Targets, ActiveTarget, PreviewBounds, Commit/CancelRequested, SnapThreshold. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Idle --> DragTracking --> Previewing --> Committing --> Idle\nDragTracking/Previewing --> Canceled --> Idle\nAny --> Failed
```

## Candidate Template Parts and Visual Tree

Candidate tree: OverlayRoot, TargetRepeater, PreviewRectangle, InvalidDropPresenter; overlay hit testing only during drag.

## Behavior and Failure Modes

Host layout model generates targets; commit is one transaction and failure restores origin. Cross-DPI/display conversion applies only within app window.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

