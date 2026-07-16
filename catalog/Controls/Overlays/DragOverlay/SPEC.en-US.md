# DragOverlay Specification

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
`DragSource`, `Preview`, `Caption`, `AllowedOperations`, `Position`, `IsVisible`; `Show/Update/Hide`. States hidden/allowed/forbidden/dropping; preview/caption/badge parts. Never hit-tests; host handles Escape.
