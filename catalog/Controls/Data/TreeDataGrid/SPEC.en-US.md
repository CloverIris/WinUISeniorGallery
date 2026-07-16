# TreeDataGrid Specification

## Goal

Define reusable responsibilities, state, and boundaries.

## Non-goals

No implementation while proposed.

## Public API

Not locked.

## State model

Not locked.

## Template parts and visual tree

Not locked.

## Behavior and failure modes

Follow referenced contracts.

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Proposed implementation baseline
`ItemsSource`, `Columns`, `SelectedItems`, `SelectionMode=Single`, `IsEditingEnabled=false`, expand/collapse, async child provider; states empty/loading/ready/editing; header/repeater/scrollbars. Left/right collapse/expand, up/down move; failures show retry row.
