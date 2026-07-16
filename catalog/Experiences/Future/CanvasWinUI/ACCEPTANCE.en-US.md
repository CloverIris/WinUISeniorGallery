# Canvas.WinUI Acceptance

## Current gate

Proposed work items must not be implemented. Chinese and English heading structures, stable IDs, and API names remain synchronized. APIs, state, errors, and performance budgets must be locked before ready.

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then and Ready gate
Given dependencies not Ready, build has no runtime type; future device loss restores focus/content, 8 ms input target, p95≤16.7 ms. Ready needs UIA/multi-DPI/leak matrix.
