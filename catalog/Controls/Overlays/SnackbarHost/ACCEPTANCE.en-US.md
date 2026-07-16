# SnackbarHost Acceptance

## Functional scenarios

- Given two windows with Hosts, when three messages target A and A is destroyed, then all A results are HostDestroyed while B queue and visual remain unchanged.
- Given pending Normal-1, Low-1, High-1, after current completion order is High-1, Normal-1, Low-1; visible work is never preempted.
- Given a pending same-key match, a new request makes the old task Replaced and keeps the queue slot. A visible match updates in place and announces once.
- Given 2s remaining, hovering 5s leaves about 2s after exit. Deactivation, focus inside, and IsPaused behave likewise.
- Given Action and timeout race in one frame, exactly one completion reason wins and the command runs at most once.
- Given Registration disposal or Host unload, timers, queue, handlers, and Composition objects clear without animation and no message migrates.

## Quality matrix

- Validate mouse, touch, keyboard, Narrator; Light/Dark/High Contrast, RTL, Reduced Motion, 100/200% DPI, and 320/800/1920 px widths.
- UIA validates polite ordinary messages, assertive Warning/Error/Critical, full Automation text despite visual trimming, and no focus stealing.
- Concurrency tests submit 1,000 requests from eight threads to same/different Hosts with no loss, duplicate completion, or cross-window display; each Host preserves stable priority.
- An empty Host has no active timer. After 10,000 completed requests, registrations, requests, and command parameters are not retained.
- Unit tests cover parameters, duration boundaries, duplicate IDs, dedup, and cancellation races. UI tests cover animation states, paused remaining time, Action/Dismiss hit targets, and destruction.
