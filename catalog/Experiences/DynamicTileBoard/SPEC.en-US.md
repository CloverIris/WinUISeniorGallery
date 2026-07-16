# DynamicTileBoard Specification

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
Tile(Id,Size,Order,Faces); tree `AdaptiveLayout→DynamicTile`; Loading/Ready/Reordering/Error; reorder commits transaction or fully rolls back.
