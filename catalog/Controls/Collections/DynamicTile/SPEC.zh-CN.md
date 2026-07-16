# DynamicTile Specification

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
API：`Faces`、`CurrentIndex`、`Size=Medium`、`Transition=Slide`、`UpdateInterval=8s`、`IsAnimationEnabled=true`、`Badge`；方法 `Advance`。状态 `Empty/Static/Animating/Paused`；部件 `PART_CurrentFace`、`PART_NextFace`、`PART_Badge`。不可见/悬停/焦点/窗口停用暂停；队列上限由 10k 次更新压力测试关闭。
