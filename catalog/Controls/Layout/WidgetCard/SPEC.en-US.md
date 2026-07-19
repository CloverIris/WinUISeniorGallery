# WidgetCard Specification

## Goal

Provide a Dashboard card with Loading/Error lifecycle, host refresh provider, auto-refresh pause, and collapsible content.

## Non-goals

No persistence, network access, window creation, or hidden interpretation of host refresh data.

## Public API

Alongside Header/Footer/Content, expose `RefreshProvider`, `RefreshAsync`, `IsAutoRefreshEnabled`, `RefreshInterval`, `IsHostVisible`, `IsHostWindowActive`, `PauseReason`, `SetPauseReason`, and refresh lifecycle events. Setting or replacing `RefreshProvider` recomputes the auto-refresh timer; stale, cancelled, or post-dispose results never overwrite newer state.

## State model

`Expanded/Collapsed/Loading/Error`; auto-refresh stops while not visible, inactive, or host-paused and waits a full interval after resuming.

## Template parts and visual tree

Not locked.

## Behavior and failure modes

Without a template the ContentControl still carries content. Refresh failure keeps old content and sets ErrorMessage; Retry uses the same provider pipeline.

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Proposed implementation baseline
API: `Header`, `Content`, `Footer`, `IsExpanded=true`, `IsCollapsible=false`, `Size=Medium`, `RefreshCommand`, `LastUpdated`. States: `Expanded/Collapsed/Loading/Error`; parts: `PART_Header`, `PART_ContentPresenter`, optional `PART_RefreshButton`. Candidate sizes Small/Medium/Wide/Large close through a WidgetsBoard prototype.
