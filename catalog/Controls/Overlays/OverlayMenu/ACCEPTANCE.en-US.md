# OverlayMenu Acceptance

## Current gate

This is an in-progress lab item: local implementation is allowed but the API is not stable. Chinese and English headings, stable IDs, and API names remain synchronized.

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Automated scenarios
Given ten nested levels When Invoke/Back occurs Then NavigationPath and CurrentItems remain aligned; Given Modal When Escape occurs Then parent navigation happens before root close; Given NonModal When an unhandled leaf is invoked Then the menu stays open; Given a disabled item When Invoke occurs Then no ItemInvoked event is raised; Given missing Scrim/BackButton When operating Then NonModal/Escape behavior remains available.
