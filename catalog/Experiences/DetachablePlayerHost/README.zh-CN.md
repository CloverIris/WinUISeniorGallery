# DetachablePlayerHost

中文规范源。当前为 `in-progress` lab，已实现播放器内容的 Inline/Detached 生命周期协调器和本地 Fake Host 演示。

## Status

in-progress / lab / P1

## Scope

控件只向宿主发出浮动窗口请求，管理操作串行化、取消、过期响应和 owner close 回退；不创建窗口、不迁移 MediaPlayer、不拥有播放会话。

## Ownership

实现：`src/WinUI3.Senior.Windowing/DetachablePlayerHost.cs`；演示：`src/WinUI3.Senior.Gallery/Pages/DetachablePlayerHostPage.xaml*`。
