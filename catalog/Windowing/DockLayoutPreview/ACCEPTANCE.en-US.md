# DockLayoutPreview Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given three targets, when pointer enters invalid space, then preview clears and layout stays. Given commit throws, original panel position/focus restores atomically.

## Input, Focus, and Cross-window Lifecycle

Mouse/touch/pen drag equals keyboard Dock commands; screen readers enumerate and confirm targets without precise pointing. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

Drag 60 Hz with hit testing under 2 ms/frame; rebuild geometry only on layout version change and closed overlay has zero hit testing.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

