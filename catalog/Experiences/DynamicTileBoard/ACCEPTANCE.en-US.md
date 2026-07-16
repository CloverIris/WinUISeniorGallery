# DynamicTileBoard Acceptance

## Current gate

Proposed work items must not be implemented. Chinese and English heading structures, stable IDs, and API names remain synchronized. APIs, state, errors, and performance budgets must be locked before ready.

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given 1,000 tiles/second updates, coalesce each≤1 Hz and animate viewport only; drag failure restores order/focus, no timer leak.
