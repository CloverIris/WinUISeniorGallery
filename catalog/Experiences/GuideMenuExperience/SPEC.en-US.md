# GuideMenuExperience Specification

## Goal

Provide an embeddable layered Guide navigation model with explicit hierarchy, breadcrumb state, and host-owned leaf actions.

## Non-goals

No window creation, host command execution, network access, or persisted navigation state.

## Public API

`GuideNode(Id, Label, Icon, Children, Tag)`; `Nodes`, `CurrentItems`, `NavigationPath`, `IsOpen`, `IsExecuting`, and `IsDismissOnLeafInvoke`. `SetNodes` normalizes and de-duplicates IDs; `Open`, `Close`, `NavigateBack`, and `Invoke` return operation results. `NodeInvoked` reports leaves, `NavigationChanged` reports hierarchy changes, and `Closed` reports dismissal.

## Template parts

`PART_Root`, `PART_Scrim`, `PART_Breadcrumb`, and `PART_Nodes`. Missing list parts still allow pure logic calls; template buttons pass stable IDs through Tag.

## Behavior

Open shows root nodes. A node with children enters the next level; a leaf raises `NodeInvoked`, and `IsDismissOnLeafInvoke` controls closing. Escape goes back first, then closes at root. Empty collections and unknown IDs return false.
