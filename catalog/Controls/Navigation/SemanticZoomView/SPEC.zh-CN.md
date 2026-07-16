# SemanticZoomView Specification

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
`ZoomedInView`, `ZoomedOutView`, `IsZoomedIn=true`, `ZoomCommand`, `MapItem`; states `ZoomedIn/Transition/ZoomedOut`, parts `PART_InPresenter`, `PART_OutPresenter`. Ctrl+滚轮、捏合和命令切换；映射失败回到组首并报告事件。
