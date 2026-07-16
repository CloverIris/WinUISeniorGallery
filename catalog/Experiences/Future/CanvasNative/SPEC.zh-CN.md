# Canvas.Native Specification

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

## 候选模型（非正式 API）
候选接口接受不可变SceneSnapshot与InputBatch，输出Frame/HitTest；状态 Uninitialized/Ready/DeviceLost/Fatal。设备丢失可重建，不改变文档。
