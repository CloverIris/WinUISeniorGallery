# Translation Provider Specification

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
候选 `TranslateAsync(segments,source,target,options,ct)` 与增量结果；状态 Unavailable/Translating/Degraded/Failed。输入SegmentId必须原样返回，乱序允许按ID重组。
