# AchievementToast Specification

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
`Title`, `Description`, `Icon`, `Progress`, `Rarity`, `Duration=6s`; `ShowAsync(target,request)`. States queued/showing/visible/closing; icon/text/progress parts. Per-window FIFO, one visible, host destruction completes HostDestroyed.
