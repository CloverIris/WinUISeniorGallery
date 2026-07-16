# ContentRail Specification

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
API: `ItemsSource`, `ItemTemplate`, `Header`, `SeeAllCommand`, `ItemSpacing=12`, `PeekExtent=48`, `PageSize=Auto`; `ScrollNext/Previous`. States: `Empty/Ready/Scrolling`; parts: `PART_Repeater`, `PART_ScrollView`, optional arrows. Vertical wheel bubbles; Shift+wheel/touch/arrows move horizontally. Close Selection via keyboard-focus/UIA prototype.
