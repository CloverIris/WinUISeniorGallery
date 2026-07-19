# WizardStepper Specification

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
`Steps`, `CurrentIndex`, `Orientation=Horizontal`, `NavigationMode=Linear`, `Next/Back/CancelCommand`, async `ValidateStep`; states `Idle/Validating/Invalid/Complete`, parts `PART_StepRepeater`, `PART_ContentPresenter`, command buttons。目标步骤必须 `IsEnabled`；验证期间防重入，失败留在原步。末步完成事件在一次流程中只发布一次；`Reset` 或返回非末步后重新武装完成事件。
