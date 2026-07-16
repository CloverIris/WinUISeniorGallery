# Canvas.WinUI Specification

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

## Candidate model (not public API)
Candidate tree `CanvasHost→NativeSurface→Selection/Adorner→InputOverlay`; Loading/Ready/Panning/Drawing/Selecting/DeviceLost. Define DP/API only after downstream ABI.
