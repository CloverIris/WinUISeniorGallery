# BigTitle Specification

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
API：`Text`、`ExpandedFontSize=64`、`CollapsedFontSize=20`、`CollapseProgress` 只读、`ScrollSource`、`IsSticky=true`。状态 `Expanded/Collapsing/Collapsed`；部件 `PART_TitlePresenter`、`PART_CompactPresenter`。进度钳制 0..1；失去 ScrollSource 回到 Expanded。最小高度在 200% 文本缩放原型无裁切后锁定。
