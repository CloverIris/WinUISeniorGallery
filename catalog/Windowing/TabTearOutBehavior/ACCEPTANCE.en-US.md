# TabTearOutBehavior Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given target ContentFactory fails, when TearOut, then source index/selection/focus stays and target closes. Given success, exactly one host owns the model at all times.

## Input, Focus, and Cross-window Lifecycle

Pointer tear-out, keyboard Move to new window, and screen-reader command are equivalent; cancel restores source, target close reattaches or closes document by host policy. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

Drag feedback 60 Hz; empty target creation target 500 ms and commit UI under 16 ms; 100 transfers leak no window/model subscription.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

