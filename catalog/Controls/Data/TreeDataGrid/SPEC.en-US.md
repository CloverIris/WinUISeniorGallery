# TreeDataGrid Specification

## Goal

Provide a host-model-driven tree grid responsible for flattening, filtering, cancellable child loading, viewport realization, sorting, and selection. Row templates, cell editors, and persistence remain host-owned.

## Non-goals

The control does not mutate `TreeDataGridNode.Value`, create windows, persist sorting/selection, or perform remote requests on behalf of a host.

## Public API

`TreeDataGridNode` exposes stable `Key`, `Value`, `Parent`, `Children`, `HasChildren`, `IsExpanded`, `LoadState`, `LoadError`, and a cancellable `ChildrenProvider`. The control exposes `SetRootItems`, `RefreshRows`, `SetViewport`, `SetExpandedAsync`, `LoadChildrenAsync`, `Select`, `ToggleSelection`, `ClearSelection`, `MoveSelection`, `SortBy`, `CommitCellEdit`, and `InvokeRow`.

`SelectionMode=Single` keeps one selection; `Multiple` supports Ctrl+Space toggling; `Extended` supports Shift+Up/Down contiguous extension. `SelectedItem` is the current anchor and `SelectedItems` is read-only. Child load states are `NotLoaded/Loading/Loaded/Failed`; cancellation or failure does not expand a node.

## State, filtering, and virtualization

`FilterText` matches `Value.ToString()` and retains a parent when any descendant matches. `VisibleRows` is the logical flattened list. `SetViewport(start,rowCount)` exposes only the viewport plus `RealizationBuffer` as `RealizedRows`; no containers are created for other rows. Sorting uses the column accessor independently at each sibling level.

## Input and failures

Up/Down moves selection, Shift extends, Left/Right collapses/expands, Enter invokes a row, and Ctrl+Space toggles multiple selection. Provider exceptions keep the node in `Failed`, set `LoadError`, and raise `ChildrenLoadFailed`; a new load cancels the old one. Empty roots show Empty and unknown operations return `false`.

## Template and accessibility

Templates may provide headers, a row repeater, scrollbars, and editors; missing optional parts do not disable model APIs. The AutomationPeer exposes DataGrid semantics, and row templates must expose hierarchy, expansion, selection, and visible focus.

## Current boundary

The item remains `in-progress/lab/P2`; column resizing, frozen columns, and in-row editors require separate specifications.
