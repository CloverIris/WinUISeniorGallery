# IconPicker Specification

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
`Sources`, `SelectedIcon`, `SearchText`, `Categories`, `Favorites`, `Recent`; states loading/ready/empty; search/category/grid/preview parts. 200 ms debounce, Enter commits, unknown glyph is non-committable placeholder.
