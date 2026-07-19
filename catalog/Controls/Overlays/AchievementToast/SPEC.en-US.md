# AchievementToast Specification

## Goal

Define a window-local FIFO celebratory toast queue. `AchievementToast` owns presentation and lifecycle; the host owns content and actions. It creates no window and persists or uploads no data.

## Non-goals

No cross-window routing, system notifications, sound, network achievement service, or persisted history.

## Public API

`AchievementToastRequest.Normalize` enforces Id/Title, Progress[0,1], and Duration 500 ms–10 min. `ShowAsync` returns `AchievementToastResult`; public operations are `Dismiss`, `InvokeAction`, and `Dispose`. States are `Queued/Showing/Visible/Closing/HostDestroyed`.

## State model

One item is visible per instance; later requests are FIFO with a default queue cap of 32. A duplicate current Id is ignored. Timer expiry enters Closing, resolves the current task, and advances the queue.

## Template parts and visual tree

`PART_Root` is required; `PART_Icon`, `PART_Title`, `PART_Description`, and `PART_Progress` are optional. Missing optional parts only disable their visual; disposal resolves queued and current work as `HostDestroyed`.

## Behavior and failure modes

There is no recoverable exception that blocks the remaining queue. A cancellation token cancels the waiter and does not retract already visible UI. Reduced Motion is handled by the theme/template without changing ordering.

## Open Decisions

Open Decisions: action-button data model, cross-window aggregation, and durable achievement history. The current implementation stays window-local.

## Proposed implementation baseline
`Title`, `Description`, `Icon`, `Progress`, `Rarity`, `Duration=6s`; `ShowAsync(target,request)`. States queued/showing/visible/closing; icon/text/progress parts. Per-window FIFO, one visible, host destruction completes HostDestroyed.
