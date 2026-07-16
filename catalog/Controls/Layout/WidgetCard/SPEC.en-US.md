# WidgetCard Specification

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
API: `Header`, `Content`, `Footer`, `IsExpanded=true`, `IsCollapsible=false`, `Size=Medium`, `RefreshCommand`, `LastUpdated`. States: `Expanded/Collapsed/Loading/Error`; parts: `PART_Header`, `PART_ContentPresenter`, optional `PART_RefreshButton`. Candidate sizes Small/Medium/Wide/Large close through a WidgetsBoard prototype.
