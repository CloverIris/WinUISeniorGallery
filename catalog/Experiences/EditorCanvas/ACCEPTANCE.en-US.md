# EditorCanvas Acceptance

## Current gate

This is an in-progress lab item: local experiments are allowed but the API is not stable. Chinese and English heading structures, stable IDs, and API names remain synchronized. 100k-object performance and persistence decisions remain before ready.

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given an empty document When the page loads Then no file access occurs; Given synthetic objects When Select and drag release Then selection moves and Revision increases; Given Rectangle/Text When drag release occurs Then an object is inserted and selected; Given Ink When pen/touch samples arrive and release occurs Then one pressure-aware CanvasStroke is committed; Given Ink When Escape/cancel occurs Then no object is inserted; Given Snap When moving Then coordinates align to GridSize; Given Delete/Undo/Redo When invoked Then only the Core snapshot changes; Given theme, RTL, or Reduced Motion changes Then the surface remains usable. Ready still requires observable 100k-object rendering/virtualization evidence.
