# MixviewExperience Specification

## Goal

提供宿主拥有节点数据的径向关联内容图。控件负责布局、选择和可访问性公告，不负责导航、推荐、网络或窗口。

## Non-goals

不实现远程推荐、图像/媒体加载、持久化布局、窗口创建或跨页导航。

## Public API

`MixNode(Id, Title, Kind, RelatedIds, Content, Tag)`；`MixviewExperience.Nodes`、`RootNodeId`、`SelectedNodeId`、`MaxVisibleNodes`（1–64，默认 12）、`IsReducedMotion`、`IsOpen`。`SetNodes` 去重并校验 ID/标题；`Open`、`Close`、`SelectNode` 返回/报告可观测结果。`NodeSelected` 标识 `IsUserInitiated`，`Closed` 仅表示宿主可收起视图。

## State model

空集合为 closed/empty；有节点且 `IsOpen=false` 为 closed；打开后为 open/selected。选择未知 ID 返回 false，不改变当前选择。`SelectedNode` 优先使用 `SelectedNodeId`，否则使用根节点。

## Template parts and visual tree

必须部件：`PART_Surface`（Canvas）；`PART_LiveRegion`（TextBlock）。缺少 Surface 时控件保持可创建但不渲染；LiveRegion 缺失只关闭公告，不影响选择。节点按钮由控件按模板主题创建，并设置 Automation Name。

## Behavior and failure modes

选择根节点后在中心显示；关联节点以确定性半径布局显示，超过 `MaxVisibleNodes` 截断。无关联 ID 时回退到其他节点。`Escape` 关闭；空集合、未知节点和无效 ID 不抛出并返回 false。Reduced Motion 只影响未来视觉转场，不改变布局/选择结果。

## Failure and degradation

节点数据无效时 `SetNodes` 抛出 `ArgumentException` 并保留旧集合；模板可选区域缺失时降级。控件绝不替宿主处理导航或内容生命周期。

## Scenario, data, and visual tree

模型为 Node → RelatedIds；树为 `Border → Canvas(PART_Surface) + LiveRegion`。同一节点顺序产生同一布局，便于 Gallery 和自动化验收。
