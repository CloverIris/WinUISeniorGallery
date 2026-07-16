# CaptionsTranslationExperience Specification

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
Session→Track→MutableSegment→Word/Translation; tree `SourceSelector→TimedTextView→Status/Controls`; Idle/Listening/Translating/Degraded/Error; monotonic revisions replace.
