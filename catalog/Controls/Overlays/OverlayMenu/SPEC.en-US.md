# OverlayMenu Specification

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
`Items`, `IsOpen`, `Modality=Modal`, `Placement=Right`, `Open/Close/NavigateBack`; states closed/opening/open/submenu; scrim/panel/items/back parts. Escape unwinds levels; Modal traps/restores focus.
