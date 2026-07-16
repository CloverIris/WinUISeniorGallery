# MiniPlayerHost Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given playback with focus on Pause, when Collapse is accepted, then playback continues, one interactive player exists, and mini Pause receives focus. Given Window close mid-transition, then no late callback and content is not disposed.

## Input, Focus, and Lifecycle

Collapse focuses the nearest semantic mini element and expand restores focus; Escape does not collapse and drag works only in DragRegion. Cover close, unload, cancel, replacement, DPI/display changes; no focus trap, late event, or leak after close.

## Performance Budget

Release x64 targets 60 Hz and under 4 ms UI/frame; never recreate MediaPlayerElement or a second session, zero subscriptions after unload.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/host rejection; UI tests themes, Reduced Motion, all input, Narrator, RTL, 100%–300% DPI, 200% text.

## Ready Promotion Acceptance

Require deterministic fake host/virtual clock, observable failures, repeatable Automation/performance; otherwise remain proposed.

