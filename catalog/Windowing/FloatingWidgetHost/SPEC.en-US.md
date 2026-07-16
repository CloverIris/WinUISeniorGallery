# FloatingWidgetHost Specification

## Goal

Create and manage app-owned secondary floating-widget windows with topmost, minimize, and owner-window lifecycle policy.

## Host and Window Boundary

Host owns secondary AppWindow and window services, while host ContentFactory creates content on target DispatcherQueue; never reparent main-window XAML instances.

## Candidate Surface and Lock Conditions

Candidate concepts: OwnerWindow, ContentFactory, WidgetId, Placement, IsAlwaysOnTop, OwnerClosePolicy, Open/Close/Restore. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Closed --> Creating --> Visible\nVisible --> Minimized/Hidden --> Visible\nCreating/Visible --> Failed\nAny --> Closing --> Closed
```

## Candidate Template Parts and Visual Tree

Per-window candidate tree: WindowChrome, WidgetTitleBar, ContentPresenter, Loading/Error presenters; owner retains only a lightweight handle/state.

## Behavior and Failure Modes

Open is idempotent per WidgetId and failure publishes no handle. Owner close follows Close/KeepAlive/Transfer; failed transfer safely closes.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

