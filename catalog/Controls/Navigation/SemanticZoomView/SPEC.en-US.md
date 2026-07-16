# SemanticZoomView Specification

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
`ZoomedInView`, `ZoomedOutView`, `IsZoomedIn=true`, `ZoomCommand`, `MapItem`; states `ZoomedIn/Transition/ZoomedOut`; parts `PART_InPresenter`, `PART_OutPresenter`. Ctrl+wheel, pinch, and command switch; failed mapping falls to group start and reports an event.
