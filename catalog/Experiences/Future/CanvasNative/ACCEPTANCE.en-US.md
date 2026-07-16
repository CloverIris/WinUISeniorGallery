# Canvas.Native Acceptance

## Current gate

Proposed work items must not be implemented. Chinese and English heading structures, stable IDs, and API names remain synchronized. APIs, state, errors, and performance budgets must be locked before ready.

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then and Ready gate
Given device loss/DPI change/bad input, no crash/document damage. Ready needs sanitizer-equivalent audit, 100k-object p95≤16.7 ms, leak test.
