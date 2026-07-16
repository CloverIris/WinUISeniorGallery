# ExpandableCommandBar Specification

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
`PrimaryCommands`, `SecondaryCommands`, `IsOpen=false`, `IsDragEnabled=true`, `Open/Close`; states `Closed/Dragging/Open`; parts primary/secondary/more/handle. Escape closes; 25% or 800 px/s commits.
