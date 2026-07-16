# DetachablePlayerHost Specification

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

## Scenario, data, and visual tree
Inline/Detaching/Detached/Attaching/Failed; tree `PlayerSlot→MediaPlayerChrome`; migrate view binding, never recreate Session. Candidate explicit `LifetimePolicy` controls window closure.
