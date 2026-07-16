# MediaCenterGrid Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given 10,000 items focused on ID 42, when refresh moves it, then only buffers are realized, focus remains 42, and it scrolls safe. Given image failure, title, invoke, and focus remain usable.

## Input, Focus, and Lifecycle

Gamepad/D-pad first, A/Enter invokes and B/Escape returns; mouse/wheel/touch/keyboard equivalent. Focus never enters unrealized items and Automation exposes row/column, title, actions. Cover close, unload, cancel, replacement, DPI/display changes; no focus trap, late event, or leak after close.

## Performance Budget

10,000 logical items realize viewport plus one screen each side; 60 Hz, under 4 ms UI/frame, under 50 ms focus feedback, no late image bind.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/host rejection; UI tests themes, Reduced Motion, all input, Narrator, RTL, 100%–300% DPI, 200% text.

## Ready Promotion Acceptance

Require deterministic fake host/virtual clock, observable failures, repeatable Automation/performance; otherwise remain proposed.

