# GameBarWidgetExperience Specification

## Goal

提供 Game Bar 风格的宿主中立 Widget 状态机和交互模式请求。

## Non-goals

不创建窗口、不设置系统点击穿透、不注册全局热键、不访问游戏/进程数据、不持久化位置。

## Public API

`WidgetId`、`PreferredSize`、`RecoveryHotKey`、`IsAlwaysOnTop`、`State`、`InteractionMode`、`AttachHost`、`DetachHost`、`OpenAsync`、`CloseAsync`、`SetInteractionMode`、`Minimize`、`Restore`。`InteractionModeRequested` 必须由宿主确认 ClickThrough 请求。

## Behavior

Open 通过 `IFloatingWidgetHost` 请求呈现；关闭/owner close 回到 Closed。ClickThrough 没有非空恢复热键或宿主未设置 `Handled=true` 时拒绝。Minimized 不销毁内容，Restore 恢复先前交互模式。
