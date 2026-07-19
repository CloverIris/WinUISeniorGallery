# ContentRail Specification

## Goal

目标：提供横向内容轨道，拥有选择、键盘/触摸方向语义和相邻预览参数；项目数据、See All 导航和业务动作由宿主拥有。

## Non-goals

不实现网络加载、图片缓存、全局导航或持久化滚动位置。

## Public API

继承 `ItemsSource`/`ItemTemplate`；公开 `Header`、`ItemWidth`、`ItemSpacing`、`PeekWidth`、`IsSnapEnabled`、`SeeAllCommand`、`PageSize`、`IsWrapNavigationEnabled`；方法 `MoveSelection`、`ScrollNext`、`ScrollPrevious`、`ScrollToIndex`、`InvokeSeeAll`。

## State model

空集合保持无选中；PageSize=0 时一次移动一个项目；RTL 镜像 Left/Right；Wrap 开启时首尾循环，否则钳制边界；Enter/Space 触发 ItemInvoked，SeeAllCommand 使用 Header 作为参数。

## Template parts and visual tree

`PART_Repeater`/`PART_ScrollView` 为主题实现可选部件；缺失时保留 ListView 默认布局和键盘逻辑。相邻预览由 ItemContainerStyle 的宽度/间距表达。

## Behavior and failure modes

滚轮事件交给父级，不拦截纵向滚动；触摸/Shift+滚轮/方向键调用同一选择状态机。ItemsSource 替换后超界选中项被修复，宿主卸载不保留计时器或事件订阅。

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Proposed implementation baseline
API：`ItemsSource`、`ItemTemplate`、`Header`、`SeeAllCommand`、`ItemSpacing=12`、`PeekExtent=48`、`PageSize=Auto`；方法 `ScrollNext/Previous`。状态 `Empty/Ready/Scrolling`，部件 `PART_Repeater`、`PART_ScrollView`、可选前后按钮。纵向滚轮向父级冒泡；Shift+滚轮/触摸/方向键横移。Selection 决定以键盘焦点与 UIA 原型关闭。
