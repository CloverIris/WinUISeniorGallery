# GameBarWidgetExperience Specification

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
WidgetSession(Bounds,Opacity,InteractionMode,Pinned); tree `FloatingHost→WidgetContent→Chrome`; Interactive/ClickThrough/Minimized/Closed; recovery hotkey required before click-through.
