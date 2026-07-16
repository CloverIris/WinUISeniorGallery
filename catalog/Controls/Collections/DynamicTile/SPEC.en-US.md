# DynamicTile Specification

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
API: `Faces`, `CurrentIndex`, `Size=Medium`, `Transition=Slide`, `UpdateInterval=8s`, `IsAnimationEnabled=true`, `Badge`; `Advance`. States: `Empty/Static/Animating/Paused`; parts: `PART_CurrentFace`, `PART_NextFace`, `PART_Badge`. Pause when hidden/hovered/focused/inactive. Close queue limit with 10k-update stress test.
