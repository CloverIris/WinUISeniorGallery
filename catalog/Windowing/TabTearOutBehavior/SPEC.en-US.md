# TabTearOutBehavior Specification

## Goal

Transactionally transfer tab data/view model from one window to a new window or another TabHost with rollback.

## Host and Window Boundary

Never reparent XAML across Windows; source/target hosts supply transferable descriptor, ContentFactory, and window factory. Behavior only coordinates transaction and focus.

## Candidate Surface and Lock Conditions

Candidate concepts: SourceHost, WindowFactory, TransferDescriptor, CanTearOut, TearOut/AttachRequested, TransferFailed. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Attached --> Dragging --> PreparingTransfer --> CreatingTarget --> Committing --> AttachedTarget\nAnyTransfer --> RollingBack --> AttachedSource\nAny --> Closed
```

## Candidate Template Parts and Visual Tree

Candidate visuals contain DragAdorner, DropTargets, and TransferPlaceholder; real content is newly created by ContentFactory on target DispatcherQueue.

## Behavior and Failure Modes

Two-phase commit removes source only after target and content are ready; any failure closes target and restores source index/selection/focus. Same-tab requests serialize.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

