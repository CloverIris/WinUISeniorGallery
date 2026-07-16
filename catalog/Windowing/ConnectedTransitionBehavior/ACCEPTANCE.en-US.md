# ConnectedTransitionBehavior Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given target in another Window, when Start, then fade, focus target, and share no Visual. Given navigation cancel, source visibility/hit/focus restores atomically.

## Input, Focus, and Cross-window Lifecycle

Proxy Visual never gets focus/hit testing; navigation target owns focus after completion. Back may use reverse configuration without blocking. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

Prepare/start each under 4 ms UI, transition 60 Hz without layout loop; candidate timeout 500 ms, 100 cancels leak no snapshot/Visual.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

