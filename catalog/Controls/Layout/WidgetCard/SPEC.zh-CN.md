# WidgetCard Specification

## Goal

目标：定义可复用职责、状态和边界。正式 API、模板部件、失败模式和性能预算尚未锁定；完成专项评审后方可进入 ready。

## Non-goals

No implementation while proposed.

## Public API

Not locked.

## State model

Not locked.

## Template parts and visual tree

Not locked.

## Behavior and failure modes

Follow referenced contracts.

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Proposed implementation baseline
API：`Header`、`Content`、`Footer`、`IsExpanded=true`、`IsCollapsible=false`、`Size=Medium`、`RefreshCommand`、`LastUpdated`。状态 `Expanded/Collapsed/Loading/Error`；部件 `PART_Header`、`PART_ContentPresenter`、可选 `PART_RefreshButton`。尺寸候选 Small/Medium/Wide/Large，由 WidgetsBoard 原型关闭。
