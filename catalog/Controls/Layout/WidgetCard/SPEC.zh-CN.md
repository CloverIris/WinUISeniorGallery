# WidgetCard Specification

## Goal

目标：提供带 Loading/Error 生命周期、宿主刷新 Provider、自动刷新暂停和可折叠内容的 Dashboard 卡片。

## Non-goals

不负责持久化、网络访问、窗口创建或替宿主解释刷新数据。

## Public API

除 Header/Footer/Content 外，提供 `RefreshProvider`、`RefreshAsync`、`IsAutoRefreshEnabled`、`RefreshInterval`、`IsHostVisible`、`IsHostWindowActive`、`PauseReason`、`SetPauseReason` 和 RefreshStarted/Completed/Failed 事件。设置或替换 `RefreshProvider` 会重新计算自动刷新计时器；刷新结果过期、取消或控件已释放时不得覆盖新状态。

## State model

`Expanded/Collapsed/Loading/Error`；不可见、窗口失活或 Host 暂停时停止自动刷新，恢复后重新等待完整间隔。

## Template parts and visual tree

Not locked.

## Behavior and failure modes

模板缺失时 ContentControl 仍可承载内容；刷新失败保留旧内容并设置 ErrorMessage，Retry 重新走同一 Provider。

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Proposed implementation baseline
API：`Header`、`Content`、`Footer`、`IsExpanded=true`、`IsCollapsible=false`、`Size=Medium`、`RefreshCommand`、`LastUpdated`。状态 `Expanded/Collapsed/Loading/Error`；部件 `PART_Header`、`PART_ContentPresenter`、可选 `PART_RefreshButton`。尺寸候选 Small/Medium/Wide/Large，由 WidgetsBoard 原型关闭。
