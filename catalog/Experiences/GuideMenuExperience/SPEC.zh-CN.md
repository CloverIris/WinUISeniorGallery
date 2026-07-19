# GuideMenuExperience Specification

## Goal

提供可嵌入的分层 Guide 导航模型，保留当前层级、面包屑和宿主叶动作边界。

## Non-goals

不创建窗口、不执行宿主命令、不访问网络、不持久化导航状态。

## Public API

`GuideNode(Id, Label, Icon, Children, Tag)`；`Nodes`、`CurrentItems`、`NavigationPath`、`IsOpen`、`IsExecuting`、`IsDismissOnLeafInvoke`。`SetNodes` 规范化并按 ID 去重；`Open`、`Close`、`NavigateBack`、`Invoke` 返回操作结果。`NodeInvoked` 只报告叶节点，`NavigationChanged` 报告层级变化，`Closed` 报告关闭。

## Template parts

`PART_Root`、`PART_Scrim`、`PART_Breadcrumb`、`PART_Nodes`。缺少列表部件时仍可调用纯逻辑 API；模板按钮通过 Tag 传递稳定 ID。

## Behavior

打开显示根节点；有子节点的调用进入下一层；叶节点触发 `NodeInvoked`，是否关闭由 `IsDismissOnLeafInvoke` 决定。`Escape` 优先返回上一层，已在根层时关闭。空集合和未知 ID 返回 false。
