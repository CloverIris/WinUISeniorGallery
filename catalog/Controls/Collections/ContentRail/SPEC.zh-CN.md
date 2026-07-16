# ContentRail Specification

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
API：`ItemsSource`、`ItemTemplate`、`Header`、`SeeAllCommand`、`ItemSpacing=12`、`PeekExtent=48`、`PageSize=Auto`；方法 `ScrollNext/Previous`。状态 `Empty/Ready/Scrolling`，部件 `PART_Repeater`、`PART_ScrollView`、可选前后按钮。纵向滚轮向父级冒泡；Shift+滚轮/触摸/方向键横移。Selection 决定以键盘焦点与 UIA 原型关闭。
