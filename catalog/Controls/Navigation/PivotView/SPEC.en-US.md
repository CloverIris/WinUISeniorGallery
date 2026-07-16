# PivotView Specification

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
`ItemsSource`, `HeaderTemplate`, `SelectedIndex`, `IsSwipeEnabled=true`, `HeaderMode=Scrollable`; states `Empty/Idle/Dragging/Settling`; parts `PART_HeaderRepeater`, `PART_ContentPresenter`, `PART_Indicator`. Keys/swipe mirror in RTL; one gesture commits one selection.
