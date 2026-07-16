# EditorCanvas Specification

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

## 场景、数据与视觉树
模型 Document→Page/Layer→Element；树 `CanvasWinUI→Rulers/Guides→ThumbnailRail`；状态 Loading/Ready/Panning/Editing/Saving/Error。当前仅场景契约，不定义Canvas API。
