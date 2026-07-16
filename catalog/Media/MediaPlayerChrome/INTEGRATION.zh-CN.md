# MediaPlayerChrome 集成

## 全局契约

- 引用 `contracts/MediaPlayback` 的 `IMediaPlaybackSession`、能力与状态类型。
- 内嵌 `MediaTimeline`，通过同一会话时间域绑定位置和范围。
- 使用全局 Theme、Motion、Input、Accessibility、Localization 和 Resources Contract。

## 服务与平台 API

宿主拥有 Window、AppWindow、页面内容、播放源和 `IMediaPlaybackSession` 生命周期。控件既不创建/关闭窗口，也不把 `MediaPlayerElement` 或任何 XAML 元素迁移到另一 XamlRoot；它只发 `PresentationRequested`，并等待宿主回写已确认模式。Window 关闭时宿主先取消请求，再释放会话与适配器。

`MediaPlayerElementPlaybackSessionAdapter` 是可选适配器，封装 `Microsoft.UI.Xaml.Controls.MediaPlayerElement` 与底层 `Windows.Media.Playback.MediaPlayer`。宿主仍负责媒体源、`SystemMediaTransportControls`、音频焦点和应用生命周期。

全屏/浮窗按钮只触发 `PresentationRequested`。宿主使用 Windowing 模块或自己的 `AppWindow` 实现，成功后回写 `IsFullScreen`/`IsCompactOverlay`。P0 对 `WinUI3.Senior.Windowing` 没有包依赖。

## 线程与生命周期

设置和事件必须在控件 DispatcherQueue 上合并。来自播放引擎的高频位置通知先节流至每秒最多 10 次 UI 更新。控件卸载或会话替换时注销事件、停止计时器并取消命令；重新加载后从当前快照恢复。

## 权限与降级

控件本身不声明网络、麦克风或视频库权限。浮窗不可用时隐藏入口；全屏未处理时保持 Inline；系统媒体控制不可用不影响本地 UI；适配器无法访问已释放播放器时进入 `Unavailable`。

## 资源与本地化

所有可见文本、Automation 名称、错误和时间格式来自资源系统。时间遵循当前区域设置；快捷键字形可按输入设备变化。不得打包第三方媒体资产。
