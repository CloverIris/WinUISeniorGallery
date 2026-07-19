# MediaTimeline 规范

## 目标

- 以同一 API 表达固定点播范围与持续移动的 Live DVR 范围。
- 将低成本拖动预览与昂贵的播放器 Seek 明确分离。
- 在一条轨道上清晰表达缓冲、章节、标记和不可跳转区域。

## 非目标

- 不直接调用播放器，不保证媒体位置更新频率。
- 不下载、解码或缓存悬停缩略图。
- 不编辑章节和标记，不承担剪辑时间轴职责。

## 公共 API

```csharp
public sealed class MediaTimeline : Control
{
    public MediaTimelineMode Mode { get; set; }
    public TimeSpan Minimum { get; set; }
    public TimeSpan Maximum { get; set; }
    public TimeSpan Position { get; set; }
    public DateTimeOffset? LiveWindowEndTime { get; set; }
    public TimeSpan LiveEdgeTolerance { get; set; }
    public double PlaybackRate { get; set; }
    public IReadOnlyList<MediaPlaybackTimeRange> BufferedRanges { get; set; }
    public IReadOnlyList<MediaPlaybackTimeRange> DisabledRanges { get; set; }
    public IReadOnlyList<MediaTimelineMarker> Chapters { get; set; }
    public IReadOnlyList<MediaTimelineMarker> Markers { get; set; }
    public bool IsSeekEnabled { get; set; }
    public TimeSpan KeyboardStep { get; set; }
    public TimeSpan LargeKeyboardStep { get; set; }
    public TimeSpan PreviewThrottleInterval { get; set; }
    public event EventHandler<MediaTimelinePreviewEventArgs>? PreviewPositionChanged;
    public event EventHandler<MediaTimelineSeekRequestedEventArgs>? SeekRequested;
    public event EventHandler? LiveEdgeRequested;
}

public enum MediaTimelineMode { VideoOnDemand, Live, LiveDvr }
public sealed record MediaTimelineMarker(string Id, TimeSpan Position, string? Label, object? Data);
```

`MediaPlaybackTimeRange` 由 `WinUI3.Senior.Core` 拥有；MediaTimeline 不定义第二套公开媒体时间范围类型。

默认值：`Mode=VideoOnDemand`、`Minimum=0`、`LiveEdgeTolerance=3s`、`PlaybackRate=1.0`、`KeyboardStep=5s`、`LargeKeyboardStep=30s`、`PreviewThrottleInterval=100ms`。`LiveWindowEndTime` 只将相对媒体时间格式化为墙钟时间，不参与 Seek 计算。`PlaybackRate` 仅用于倍速标签和 Automation 值文本，不缩放媒体时间、步进或 Seek；非有限值或小于等于 0 的值按 1.0 呈现。

事件参数必须含请求位置、是否由用户发起、输入类型；`SeekRequested` 还含 `CancellationToken` 不适用，事件本身同步发出，由宿主异步执行并通过 `Position` 反馈结果。

## 时间域与归一化

所有位置使用同一 `TimeSpan` 媒体时间域，闭区间为 `[Minimum, Maximum]`。`Maximum < Minimum` 时呈现不可用态。范围先裁剪到可用区间；`End <= Start` 的范围忽略；缓冲与禁用范围分别排序并合并重叠项。章节与标记按位置稳定排序，区间外项不呈现但不修改输入集合。

落入禁用区间的拖动位置按最近边界修正；距离相同则按用户拖动方向选择。键盘前进/后退跳过整个禁用区间。宿主写入禁用位置时只在显示层钳制，不反向发 Seek。

## 状态模型

状态为 `Unavailable`、`ReadOnly`、`Idle`、`PointerOver`、`Scrubbing`、`AtLiveEdge`。`Live` 模式不可 Seek，只显示当前直播与 Live 标识。`LiveDvr` 的 `Maximum` 是动态 Live Edge；`Position >= Maximum - LiveEdgeTolerance` 进入 `AtLiveEdge`。

拖动开始时冻结一次输入范围快照；拖动期间最多按 `PreviewThrottleInterval` 发出 `PreviewPositionChanged`，最后一个预览不得丢失。释放时恰好发出一次 `SeekRequested`。取消拖动不发 Seek，并恢复 `Position`。方向键、PageUp/PageDown、Home/End 每次都立即发出一次 Seek；Live DVR 的 End 发 `LiveEdgeRequested`，宿主未处理时再请求当前 `Maximum`。

## 模板部件

状态转换固定为：

```text
Unavailable --> ReadOnly | Idle
Idle --> PointerOver --> Scrubbing
Scrubbing --> Idle          (commit or cancel)
Idle/PointerOver --> AtLiveEdge
AtLiveEdge --> Idle         (position leaves tolerance)
Any --> Unavailable         (invalid range or unload)
```

- `PART_RootGrid`：根布局。
- `PART_Track`：可用范围轨道。
- `PART_BufferedRangesPresenter`、`PART_DisabledRangesPresenter`：区间层。
- `PART_ChapterPresenter`、`PART_MarkerPresenter`：标记层。
- `PART_Progress`、`PART_Thumb`：位置与拖动。
- `PART_ToolTip`、`PART_TimeText`、`PART_PlaybackRateText`、`PART_LiveBadge`、`PART_GoLiveButton`：反馈、倍速与 Live 操作。

除 `PART_RootGrid` 外缺失部件仅关闭对应视觉。视觉状态组：`CommonStates`、`InteractionStates`、`TimelineModeStates`、`LiveStates`。

## 失败模式

- 集合为空、被并发替换或包含无效项时不得崩溃，采用最新完整快照。
- 控件卸载时取消待发预览，不得在卸载后发 Seek。
- 动态窗口移动越过当前拖动位置时，最终位置重新钳制到释放时的最新有效范围。
- 宿主拒绝 Seek 时只需保持或回写原 `Position`；控件不得假定成功。
