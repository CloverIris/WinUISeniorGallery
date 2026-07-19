# PropertyGrid Specification

## Goal

Provide reflection metadata, host editors, validation, and reversible property transactions. The host owns the selected object's lifetime.

## Non-goals

No object creation, configuration persistence, network access, or hidden custom-editor implementation.

## Public API

`SelectedObject`, `Properties`, `Groups`, `FilterText`, `SortMode`, `IsReadOnly`, `EditorProvider`, `BeginEdit`, `CommitEdit`, `CancelEdit`, `TrySetValue`, `Undo`, `Redo`, `ClearHistory`, `CanUndo`, and `CanRedo`. A successful edit enters Undo; a new edit clears Redo.

## State model

`Empty/Ready/Editing/Error`. Validation failure preserves the model value; Undo/Redo run through the same conversion and validation pipeline and leave history unchanged on failure.

## Template parts and visual tree

Not locked.

## Behavior and failure modes

Pure logic remains usable without a template. Host templates bind editors to `PropertyGridProperty.Value` and `ValidationError`; Automation reports the current object, property count, and Undo/Redo availability.

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Proposed implementation baseline
`SelectedObject`, `Properties`, `EditorProvider`, `SortMode=Categorized`, `FilterText`, `IsReadOnly`; states empty/ready/editing/error; toolbar/tree/description/editor parts. Enter commits, Escape rolls back; validation failure does not write model.
