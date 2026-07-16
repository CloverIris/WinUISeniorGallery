# WindowChrome Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given a title-bar search box, when resize occurs, then search remains interactive and remaining background draggable. Given unsupported backdrop, solid fallback retains system menu/Snap.

## Input, Focus, and Cross-window Lifecycle

Drag regions contain only noninteractive background; Alt+Space, double-click maximize, Snap Layout, keyboard, and touch retain system semantics. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

At resize/DPI, recompute at most once/frame under 2 ms UI work; 100 Attach/Detach cycles leak no event or backdrop controller.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

