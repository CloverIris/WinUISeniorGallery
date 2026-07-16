# RevealBorderBehavior Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given keyboard focus with no mouse, then FocusVisual is clear without Reveal. Given High Contrast, dynamic light is off and static border meets system contrast.

## Input, Focus, and Cross-window Lifecycle

Mouse/pen hover gets light and touch press feedback; keyboard/gamepad always has independent standard FocusVisual and never relies on Reveal. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

One shared light/Window; at 500 visible elements UI under 2 ms/frame with no layout, invisible elements zero updates, unload releases Composition refs.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

