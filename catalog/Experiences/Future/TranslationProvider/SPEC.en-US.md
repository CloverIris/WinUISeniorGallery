# Translation Provider Specification

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

## Candidate model (not public API)
Candidate `TranslateAsync(segments,source,target,options,ct)` plus incremental results; Unavailable/Translating/Degraded/Failed. Preserve SegmentId; reorder by ID.
