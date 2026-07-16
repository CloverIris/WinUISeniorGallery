# SettingsPanel Specification

## Goal

Provide an in-window settings side panel with hierarchical navigation, back, and unsaved-change policy.

## Host and Window Boundary

Create no settings window, persist no settings, and decide no identity/permission; host supplies page factory, navigation stack, commit/cancel policy.

## Candidate Surface and Lock Conditions

Candidate concepts: IsOpen, RootPage, NavigationStack, PaneWidth, IsModal, Navigate/Back/CloseRequested, UnsavedChangesPolicy. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Closed --> Opening --> Root\nRoot <--> Child\nRoot/Child --> ConfirmingChanges --> Closing --> Closed\nAny --> Failed
```

## Candidate Template Parts and Visual Tree

Candidate tree: Scrim, PaneRoot, Header, BackButton, FramePresenter, Footer, FocusSentinels, ErrorPresenter.

## Behavior and Failure Modes

Navigation is async/cancellable; repeated open preserves stack or resets by host policy. Close with dirty state confirms first and prevents duplicate close.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

