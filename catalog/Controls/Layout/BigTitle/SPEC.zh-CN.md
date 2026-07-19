# BigTitle Specification

## Goal

目标：提供滚动驱动的 Metro/Fluent 大标题收缩控件。控件只同步标题视觉，不拥有页面导航或 ScrollViewer 生命周期。

## Non-goals

不实现导航、路由、持久化、自动创建 ScrollViewer 或页面重建。

## Public API

`Text`、`ExpandedFontSize`、`CollapsedFontSize`、`CollapseDistance`、只读 `CollapseProgress`、`IsSticky`、`ScrollSource`；只读 `State` 为 Expanded/Collapsing/Collapsed。ScrollSource 为空时进度回到 0。

## State model

VerticalOffset/CollapseDistance 钳制到 0..1；文本/字体变化立即刷新；窗口卸载解除 ViewChanged，避免页面离开后继续回调。

## Template parts and visual tree

`PART_TitlePresenter` 与 `PART_CompactPresenter` 为可选部件；缺失部件只禁用对应层，不阻止状态计算。标题字号线性插值，Expanded 层透明度随进度下降，Compact 层随进度上升。

## Behavior and failure modes

ScrollSource 失败或不可用时保持 Expanded；Reduced Motion 不影响状态和任务完成，只由模板去除额外动画。

## Open Decisions

Open Decisions：文本缩放 200% 下的最小高度、Sticky 视觉的宿主布局约束和 RTL 标题对齐策略。

## Proposed implementation baseline
API：`Text`、`ExpandedFontSize=64`、`CollapsedFontSize=20`、`CollapseProgress` 只读、`ScrollSource`、`IsSticky=true`。状态 `Expanded/Collapsing/Collapsed`；部件 `PART_TitlePresenter`、`PART_CompactPresenter`。进度钳制 0..1；失去 ScrollSource 回到 Expanded。最小高度在 200% 文本缩放原型无裁切后锁定。
