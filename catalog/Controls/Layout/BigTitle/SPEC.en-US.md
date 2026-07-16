# BigTitle Specification

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
API: `Text`, `ExpandedFontSize=64`, `CollapsedFontSize=20`, read-only `CollapseProgress`, `ScrollSource`, `IsSticky=true`. States: `Expanded/Collapsing/Collapsed`; parts: `PART_TitlePresenter`, `PART_CompactPresenter`. Clamp progress 0..1; missing source returns Expanded. Lock minimum height after a no-clipping 200% text prototype.
