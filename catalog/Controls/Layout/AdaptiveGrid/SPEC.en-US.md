# AdaptiveGrid Specification

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
Candidate API: `ItemsSource`, `ItemTemplate`, `MinItemWidth=160`, `MaximumColumns=0` (unlimited), `HorizontalSpacing=12`, `VerticalSpacing=12`, `StretchItems=true`. States: `Empty/Measuring/Ready`; required `PART_Repeater:ItemsRepeater`. Coalesce resize to one layout per frame; invalid width/spacing throws `ArgumentOutOfRangeException`. Masonry closes by prototype meeting virtualization and spatial-keyboard budgets, otherwise leaves v1.
