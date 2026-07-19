# OverlayMenu Specification

## Goal

Provide a hierarchical overlay menu without leaving the current content context. The menu owns open state and navigation stack; the host owns item data and final actions.

## Non-goals

No window creation, system-level Charms behavior, route persistence, or cloud commands.

## Public API

`OverlayMenu.Items`, `IsOpen`, `Modality (Modal/NonModal)`, `Placement (Right/Left/Bottom/Center)`, and `IsBackButtonVisible`; methods `Open`, `Close`, `NavigateBack`, and `Invoke`; events `Opened`, `Closed`, `ItemInvoked`, and `NavigationChanged`.

## State model

The root opens with an empty path. An item with Children enters a submenu; a leaf raises the event. A Modal leaf closes unless the event is handled; NonModal remains open. Escape returns to the parent first and closes at the root.

## Template parts and visual tree

`PART_Panel` and `PART_Items` are required; `PART_Scrim` and `PART_BackButton` are optional. Without Scrim, NonModal usage still works; without BackButton, Escape navigation remains available.

## Behavior and failure modes

Empty Items may open and show an empty state; disabled items do not raise events. Hosts can replace Items and call Open to reread the root. The control does not take ownership of focus outside its content.

## Open Decisions

Open Decisions: exact focus-loop automation, narrow-window fallback for Bottom/Center placement, and deep-navigation animation duration.

## Proposed implementation baseline
`Items`, `IsOpen`, `Modality=Modal`, `Placement=Right`, `Open/Close/NavigateBack`; states closed/opening/open/submenu; scrim/panel/items/back parts. Escape unwinds levels; Modal traps/restores focus.
