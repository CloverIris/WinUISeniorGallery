# MediaPlayerChrome 规范

## 目标

- 用同一控制层适配系统与第三方播放引擎。
- 对点播、直播和 Live DVR 显示一致且可访问的操作。
- 让宿主明确处理全屏和浮窗请求，不隐式操纵窗口。

## 非目标

- 不解析媒体、不持有媒体源、不选择音轨。
- 不创建 `AppWindow`，不调用 `CompactOverlayPresenter`，不迁移 XAML 内容。
- 不在 P0 实现播放列表、投屏或 ASR。

## 公共 API

```csharp
public sealed class MediaPlayerChrome : Control
{
    public IMediaPlaybackSession? PlaybackSession { get; set; }
    public MediaChromeDisplayMode DisplayMode { get; set; }
    public bool IsAutoHideEnabled { get; set; }
    public TimeSpan AutoHideDelay { get; set; }
    public bool IsCompactOverlayAvailable { get; set; }
    public bool IsFullScreen { get; set; }
    public bool IsCompactOverlay { get; set; }
    public event EventHandler<MediaChromePresentationRequestedEventArgs>? PresentationRequested;
}

public enum MediaChromeDisplayMode { Full, Compact, Minimal }
public enum MediaChromePresentationMode { Inline, FullScreen, CompactOverlay }

public sealed class MediaChromePresentationRequestedEventArgs : EventArgs
{
    public MediaChromePresentationMode RequestedMode { get; }
    public bool Handled { get; set; }
}

public sealed class MediaPlayerElementPlaybackSessionAdapter : IMediaPlaybackSession, IDisposable
{
    public MediaPlayerElementPlaybackSessionAdapter(MediaPlayerElement element);
}
```

`IMediaPlaybackSession` 是共享契约，必须提供：只读播放状态、能力、位置、时间范围、音量和倍速，以及异步 `Play`、`Pause`、`Stop`、`Seek`、`SetVolume`、`SetPlaybackRate` 命令；状态变化必须通过可注销事件或可观察通知发出。所有异步命令接受 `CancellationToken`。

属性规则：`PlaybackSession` 可为空；空值显示禁用态。`AutoHideDelay` 小于 1 秒时按 1 秒处理。`IsFullScreen` 与 `IsCompactOverlay` 是宿主确认后的事实状态，不由请求事件自动改变。

## 状态模型

控件把会话映射为 `Unavailable`、`Loading`、`Ready`、`Playing`、`Paused`、`Buffering`、`Ended`、`Failed`。会话替换时立即注销旧会话、取消旧命令并刷新全部状态。命令执行期间只禁用冲突操作；失败后恢复可操作状态并显示非阻塞错误。

自动隐藏只在 `Playing` 且指针不在控件内、无键盘焦点、无菜单打开、未拖动时间轴时生效。触摸、鼠标移动、键盘或手柄输入应重新显示控件并重置计时器。

## 模板部件

状态转换固定为：

```text
Unavailable --> Loading --> Ready
Ready/Paused/Ended --> Playing
Playing --> Paused | Buffering | Ended
Buffering --> Playing | Paused | Failed
Any session state --> Failed | Unavailable

Presentation (orthogonal, host-confirmed):
Inline <--> FullScreen
Inline <--> CompactOverlay
```

- `PART_RootGrid`：根视觉层。
- `PART_Timeline`：`MediaTimeline`。
- `PART_PlayPauseButton`、`PART_StopButton`：传输命令。
- `PART_VolumeSlider`、`PART_MuteButton`：音量。
- `PART_PlaybackRateButton`：倍速菜单。
- `PART_FullScreenButton`、`PART_CompactOverlayButton`：仅发出请求。
- `PART_BufferingIndicator`、`PART_ErrorPresenter`：反馈。
- `PART_MoreButton`：紧凑模式溢出入口。

缺失非根模板部件时降级隐藏对应能力，不得崩溃。视觉状态组：`PlaybackStates`、`DisplayModeStates`、`VisibilityStates`、`PresentationStates`、`CommonStates`。

## 行为与失败模式

- 连续快速操作必须串行化同类命令；后一次 Seek 可以取消尚未提交的前一次 Seek。
- 不支持的能力不显示或禁用，并提供 Automation 帮助文本。
- 会话抛出异常时捕获并映射为可本地化错误，不向 UI 线程传播。
- 宿主未处理 `PresentationRequested` 时保持原状态；控件不得猜测窗口结果。
- Adapter 在 UI 线程读取 `MediaPlayerElement`，释放后不再转发事件。
