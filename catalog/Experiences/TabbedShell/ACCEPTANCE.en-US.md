# TabbedShell Acceptance

## Current gate

Proposed work items must not be implemented. Chinese and English heading structures, stable IDs, and API names remain synchronized. APIs, state, errors, and performance budgets must be locked before ready.

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given create/save/close races, tab has one WindowId and dirty data persists; 100 tabs virtualize, restore p95≤1 s.
