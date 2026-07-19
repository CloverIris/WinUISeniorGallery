# BigTitle

Specification work item for BigTitle.

## Status

in-progress / lab / P2. Scroll-driven collapse logic and dual presenter template are implemented locally.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Controls/Layout/BigTitle

## Implementation readiness
The host explicitly supplies `ScrollSource`; VerticalOffset maps to 0..1 through CollapseDistance and Expanded/Collapsing/Collapsed only affect title visuals. Navigation remains host-owned.
