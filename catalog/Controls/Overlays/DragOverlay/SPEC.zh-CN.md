# DragOverlay Specification

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
`DragSource`, `Preview`, `Caption`, `AllowedOperations`, `Position`, `IsVisible`; methods `Show/Update/Hide`. states `Hidden/Allowed/Forbidden/Dropping`, parts `PART_Preview`, `PART_Caption`, `PART_Badge`. 不参与命中测试，Esc取消由宿主处理。
