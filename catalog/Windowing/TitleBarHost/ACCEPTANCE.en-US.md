# TitleBarHost Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given three RightContent buttons, when RTL and 200% text scale, then buttons focus, regions avoid caption buttons, and background remains draggable.

## Input, Focus, and Cross-window Lifecycle

Tab enters interactive content only, never drag background; preserve caption order, RTL, Alt+Space, and title-bar double-click. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

Coalesce layout into one RegionsChanged/frame under 2 ms; frequent title updates never rebuild the visual tree.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

