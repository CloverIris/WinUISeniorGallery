# EdgeCommandPanel Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given half-open nonmodal panel, when Window narrows, then extent clamps and content remains focusable. Given capture loss, panel settles to one stable state and announces.

## Input, Focus, and Cross-window Lifecycle

Explicit button, keyboard, and touch handle open; Escape/B closes innermost, and edge gesture exists only in app-declared zone without system conflict. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

Drag at 60 Hz under 3 ms UI/frame; 100 commands virtualized via ItemsRepeater and no pointer capture after close.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

