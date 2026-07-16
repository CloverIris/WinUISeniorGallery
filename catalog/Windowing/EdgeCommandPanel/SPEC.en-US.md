# EdgeCommandPanel Specification

## Goal

Present a draggable command panel at an app-window edge, preserving progressive disclosure without imitating system edge UI.

## Host and Window Boundary

Work only inside host XAML root; register no global edge gesture, cover no other app, and call no system Charms. Host supplies commands and opening policy.

## Candidate Surface and Lock Conditions

Candidate concepts: Edge, ItemsSource, IsOpen, PeekExtent, ExpandedExtent, IsModal, Open/CloseRequested. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Closed --> Peeking --> Dragging --> Open\nOpen --> Dragging --> Closed\nAny --> Failed/Unloaded
```

## Candidate Template Parts and Visual Tree

Candidate tree: Scrim, PanelRoot, DragHandle, CommandItemsRepeater, Header/FooterPresenter, FocusSentinel.

## Behavior and Failure Modes

Drag settles by threshold/velocity; modal traps focus while nonmodal preserves content. Resize recomputes extents and clamps progress.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

