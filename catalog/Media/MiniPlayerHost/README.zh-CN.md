# MiniPlayerHost

定义同一 XAML Window 内把完整播放器折叠为边缘迷你播放器的宿主，保持同一播放会话、焦点恢复点与内容状态。

## 状态与范围

- 状态：in-progress / lab / P1
- 依赖：media.media-player-chrome
- 当前已提供不创建窗口的紧凑宿主、播放/暂停、恢复与关闭请求；主题和最终验收仍在推进。

## 宿主边界

只管理当前窗口内两个呈现槽位；不创建 AppWindow、不请求 CompactOverlay、不跨窗口重父级化 XAML。窗口化交给 CompactOverlayHost/DetachablePlayerHost，播放命令由 MediaPlayerChrome 和 IMediaPlaybackSession 拥有。

## 文档与 Agent 所有权

SPEC 定义晋级门禁，DESIGN 覆盖视觉/输入，INTEGRATION 覆盖生命周期，ACCEPTANCE 给出 Given/When/Then。实现位于 `src/WinUI3.Senior.Media/MediaPlayerChrome/MiniPlayerHost.cs`。
