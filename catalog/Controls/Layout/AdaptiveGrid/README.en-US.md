# AdaptiveGrid

Specification work item for AdaptiveGrid. A deterministic layout calculator is now available.

## Status

in-progress / lab / P1.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Controls/Layout/AdaptiveGrid

## Implementation readiness
Virtualizes arbitrary items into adaptive equal-width columns in `WinUI3.Senior.Controls`. Before Ready, decide whether Masonry ships in v1; default is excluded.

`AdaptiveGridLayoutCalculator` exposes the same column, row, and item-width calculation so HomeScreen/Dashboard hosts can preview layout before creating visual containers.

The control raises `LayoutChanged` only when the measured or arranged result changes. Its event argument carries an immutable `AdaptiveGridLayoutResult`, so repeated Measure calls do not create an event storm.
