# DetachablePlayerHost Specification

## Goal

在不耦合具体播放器引擎的前提下，提供 Inline、Detaching、Detached、Attaching、Failed 状态机和宿主拥有的浮动请求。

## Non-goals

不创建 `Window`/`AppWindow`，不迁移或复制播放会话，不持久化文件路径，不执行网络或媒体命令。

## Public API

`PlayerId`、`PreferredSize`（默认 480×270，限制 160–4096×90–4096）、`OwnerClosePolicy`、`IsAlwaysOnTop`、`State`、`ActiveRequest`、`AttachHost`、`DetachHost`、`DetachAsync`、`AttachAsync`、`ToggleAsync`。操作返回 Success/Rejected/Cancelled/Failed，并带稳定错误码。

## State and behavior

Detach 只允许从 Inline/Failed 发起；Attach 只允许从 Detached 发起。每次操作通过单一信号量串行，host 替换、owner close、卸载或取消会使迟到响应失效。owner close 立即回到 Inline 并触发 `HostClosed`。

## Host boundary

`IFloatingWidgetHost.OpenAsync` 接收宿主内容引用和请求；关闭、创建、窗口迁移和生命周期均由宿主决定。控件保留 `Content`，不假设外部窗口已经成功承载它。
