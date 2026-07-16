# GuideMenuExperience Specification

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
模型 GuideNode(Id,Label,Icon,Children,Command)；树 `EdgePanel→Breadcrumb→NodeList`；状态 Closed/Root/Submenu/Executing/Error，命令执行不隐式关闭除非策略指定。
