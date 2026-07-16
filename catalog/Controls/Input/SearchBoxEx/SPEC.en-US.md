# SearchBoxEx Specification

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
`Text`, `QueryCommand`, `SuggestionProvider`, `IsHistoryEnabled=false`, `Debounce=250ms`, `MinimumPrefixLength=1`; states idle/loading/results/error; box/list/progress parts. Arrows preview, Enter commits, Escape closes; stale requests never win.
