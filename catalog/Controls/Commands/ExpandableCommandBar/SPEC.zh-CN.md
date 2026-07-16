# ExpandableCommandBar Specification

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
`PrimaryCommands`, `SecondaryCommands`, `IsOpen=false`, `IsDragEnabled=true`, `Open/Close`; states `Closed/Dragging/Open`, parts `PART_Primary`, `PART_Secondary`, `PART_MoreButton`, `PART_DragHandle`. Esc关闭，拖过25%或800px/s提交。
