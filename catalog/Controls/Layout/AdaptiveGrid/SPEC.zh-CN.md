# AdaptiveGrid Specification

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
候选 API：`ItemsSource`、`ItemTemplate`、`MinItemWidth=160`、`MaximumColumns=0`（不限）、`HorizontalSpacing=12`、`VerticalSpacing=12`、`StretchItems=true`。状态 `Empty/Measuring/Ready`；必需部件 `PART_Repeater:ItemsRepeater`。尺寸变化按帧合并一次布局，非法宽度/间距抛 `ArgumentOutOfRangeException`。Masonry 决定关闭条件：原型证明虚拟化和键盘空间导航均达预算，否则移出 v1。
