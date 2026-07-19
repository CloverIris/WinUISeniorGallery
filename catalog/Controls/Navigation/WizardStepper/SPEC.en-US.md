# WizardStepper Specification

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
`Steps`, `CurrentIndex`, `Orientation=Horizontal`, `NavigationMode=Linear`, commands, async `ValidateStep`; states `Idle/Validating/Invalid/Complete`; parts repeater/content/buttons. A target step must be `IsEnabled`; validation blocks reentry and failure stays put. The final `Completed` event is emitted once per flow, and is re-armed by `Reset` or moving away from the final step.
