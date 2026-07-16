# ColorPickerEx Specification

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
`Color`, `ColorSpace=Rgb`, `IsAlphaEnabled=true`, `HexText`, `Palette`, `History`, `EyedropperProvider`; states valid/invalid/picking; spectrum/sliders/text/palette parts. Blur commits; invalid text keeps old Color and shows error.
