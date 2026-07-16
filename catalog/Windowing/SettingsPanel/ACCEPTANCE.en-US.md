# SettingsPanel Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given dirty child page, when Escape, then confirm once. Given Window close, then cancel navigation/save without blocking Closed.

## Input, Focus, and Cross-window Lifecycle

Alt+Left/Backspace/B navigates back and Escape closes inner popup then panel; modal focus is contained, close restores trigger, errors focus first invalid field. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

First open frame under 100 ms and page switch under 150 ms; close releases page subscriptions and host policy freezes cache cap.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

