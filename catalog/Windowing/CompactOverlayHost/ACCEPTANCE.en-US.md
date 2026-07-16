# CompactOverlayHost Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given overlay unsupported, when enter is requested, then fail and retain Inline/size/focus. Given Window close while Entering, then do not restore a closed AppWindow.

## Input, Focus, and Cross-window Lifecycle

Overlay always has keyboard/touch/mouse exit; host policy owns Escape and it never closes Window; focus remains in visible content. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

Switch never rebuilds root visual tree, confirmation target under 250 ms; 100 switches leak no presenter/event.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

