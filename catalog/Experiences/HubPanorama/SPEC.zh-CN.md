# HubPanorama Specification

## Goal

目标：提供 Metro Hub 风格的横向章节探索。控件拥有 Section 选择、滚动对齐和方向键语义，宿主拥有内容、背景和路由。

## Non-goals

不实现网络素材、窗口创建、路由持久化或 Pivot API 复制。

## Public API

`HubSection`（Id/Header/Items/Background）、`Sections`、`SelectedIndex`、`SectionWidth`、`IsParallaxEnabled`、`ParallaxStrength`、`IsWrapNavigationEnabled`、`IsReducedMotion`；方法 `SetSections`、`SelectSection`、`Navigate`；事件 `SectionChanged`。

## State model

空集合 SelectedIndex=-1；Section 以横向滚动排列；Left/Right 按 RTL 镜像，Home/End 跳到首尾；Wrap 开启时循环，否则钳制。滚动 offset 变化会修复当前 Section。

## Template parts and visual tree

必需 `PART_ScrollViewer`、`PART_Repeater`；可选 `PART_Indicator`。缺失 Indicator 不影响 Section 选择和滚动；模板不要求背景对象为特定类型。

## Behavior and failure modes

Section Id 重复时只保留首次；SectionWidth 钳制 240–2000；Reduced Motion 禁用 ChangeView 动画但不改变选择结果。宿主卸载后解除 ScrollViewer 事件。

## Open Decisions

Open Decisions：窄窗口纵向回退、背景视差实际渲染管线和 10k Section 虚拟化预算。

## 场景、数据与视觉树
模型 Section(Id,Header,Template,Items,BackgroundLayer)；树 `HubRoot→SectionRepeater`，状态 Empty/Ready/Panning/Settling；当前 Section 是路由书签，不复制 Pivot API。
