# TabbedShell Specification

## Goal

Provide a window-neutral tab container for selection, cancellable close, reorder and host-owned tear-out requests.

## Non-goals

It does not create or move a Window, restore a crashed process, persist tab content, or execute business navigation.

## Public API

`TabbedShellItem(Id, Header, Content, IconGlyph, CanClose, IsPinned)`; a window-local `Items` collection; bindable `SelectedItem`; and `AddTab`, `SelectTab`, `CloseTab`, `MoveTab`, `SelectNext`, `RequestTearOut`. Events are `SelectionChanged`, cancellable `TabClosing`, `TabClosed`, `TabReordered`, and `TearOutRequested`.

## State model

`Empty` and `HasSelection`; a close request is cancellable before removal. A host may represent tear-out as Transferring, but the control does not mutate the collection on its own.

## Template parts and visual tree

Required `PART_TabList` (ListView) and `PART_ContentPresenter` (ContentPresenter). If either part is absent the data API remains usable, but visual presentation is not guaranteed.

## Behavior and failure modes

Ctrl+Tab/Shift+Ctrl+Tab cycles selection, Ctrl+W requests close, Ctrl+T requests tear-out, and Ctrl+1…9 selects the first nine tabs. Duplicate/unknown ids, non-closable tabs and cancelled `TabClosing` leave the collection unchanged; closing the last tab sets `SelectedItem` to null.

## Open Decisions

Remaining review decisions are cross-window restore, crash recovery, tear-out failure rollback and persistence. They do not block the local laboratory implementation.

## Scenario, data, and visual tree
Tab(Id,Title,ContentKey,Dirty,WindowId); tree `TabStrip→ContentHost`; Empty/Ready/Dragging/Transferring/Closing; close cancellable, transfer atomic.
