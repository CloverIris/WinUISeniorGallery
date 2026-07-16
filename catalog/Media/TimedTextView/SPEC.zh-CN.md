# TimedTextView 规范

## 目标

- 用稳定、与 Provider 无关的数据模型表达文件字幕、歌词和实时 ASR。
- 在同一控件中提供单行、滚动、逐词高亮和双语显示。
- 对大文档使用虚拟化，并在实时修订时保持阅读位置与辅助技术稳定。

## 非目标

- 不识别语音、不调用翻译 API、不保存凭据。
- 不解析 SRT、VTT、LRC、TTML；解析器作为独立服务。
- 不控制播放器，不修改播放位置；点击文本仅发请求事件。

## 公共 API

```csharp
public sealed class TimedTextView : Control
{
    public TimedTextDocument? Document { get; set; }
    public string? ActiveTrackId { get; set; }
    public TimeSpan Position { get; set; }
    public TimeSpan TimingOffset { get; set; }
    public TimedTextDisplayMode DisplayMode { get; set; }
    public bool IsAutoScrollEnabled { get; set; }
    public int ContextLineCount { get; set; }
    public event EventHandler<TimedTextActiveSegmentChangedEventArgs>? ActiveSegmentChanged;
    public event EventHandler<TimedTextSegmentInvokedEventArgs>? SegmentInvoked;
}

public enum TimedTextDisplayMode { SingleLine, ScrollingLyrics, Karaoke, Bilingual }
public enum TimedTextRevisionState { Interim, Final }
public enum TimedTextTrackRole { Captions, Subtitles, Lyrics, Translation, Transcript }

public sealed record TimedTextDocument(
    string Id, long Revision, IReadOnlyList<TimedTextTrack> Tracks);
public sealed record TimedTextTrack(
    string Id, string Language, TimedTextTrackRole Role,
    IReadOnlyList<TimedTextSegment> Segments);
public sealed record TimedTextSegment(
    string Id, TimeSpan Start, TimeSpan End, string Text,
    string? TranslatedText, TimedTextRevisionState RevisionState,
    IReadOnlyList<TimedTextWord> Words);
public sealed record TimedTextWord(
    string Id, TimeSpan Start, TimeSpan End, string Text,
    string? TranslatedText, TimedTextRevisionState RevisionState);
```

默认值：`DisplayMode=SingleLine`、`TimingOffset=0`、`IsAutoScrollEnabled=true`、`ContextLineCount=2`。有效查询时间为 `Position + TimingOffset`。`ContextLineCount` 钳制到 0–10。

事件参数包含文档、轨道、Segment、查询位置和用户发起标志。`SegmentInvoked` 仅表达导航意图，宿主决定是否 Seek。

## 数据与修订规则

Document、Track、Segment、Word 是不可变快照。相同 Document `Id` 的 `Revision` 必须单调递增；等于或低于当前 Revision 的快照忽略。Document `Id` 改变时允许 Revision 重新开始。Segment/Word `Id` 在一个 Document 内稳定；Interim 可被同 ID 后续快照替换，Final 也只由更高 Document Revision 替换。删除通过新快照中不再包含该 ID 表达。

控件选择 `ActiveTrackId`；为空或不存在时选第一条与当前 UI 语言最匹配的轨道，否则选第一条。Bilingual 优先使用同一 Segment 的 `TranslatedText`；为空时查找时间重叠且 Role 为 Translation 的轨道；仍为空则只显示原文，不显示空占位。

无效规则：`End <= Start` 的 Segment/Word 忽略；Word 必须裁剪到父 Segment 时间范围；空 `Id` 或重复 `Id` 保留首次出现项；Segment 按 Start、End、输入顺序稳定排序。不得修改输入集合。

## 活动文本状态

时间区间采用 `[Start, End)`。多个 Segment 重叠时选择 Start 最晚者，再按排序后的首项决定；其他重叠项仍可在滚动模式显示。查询时间处于间隙时 SingleLine/Karaoke 显示空，ScrollingLyrics/Bilingual 保留上下文但无活动高亮。

Karaoke 在活动 Segment 内以同样规则选择活动 Word；未提供有效 Word 时退化为整段高亮。`ActiveSegmentChanged` 只在活动 ID 改变时发出，位置高频变化不得重复发同一 ID。

用户手动滚动后暂停自动居中 5 秒；再次调用滚动或激活新的远离视口 Segment 后恢复。实时修订不得抢走键盘焦点；同 ID 高度变化应锚定当前活动项。

## 模板部件

内容与交互状态固定为：

```text
Empty --> Indexing --> Ready
Ready --> ActiveSegmentChanged --> Ready
Ready --> UserScrolling --> AutoScrollSuspended --> Ready
Ready --> ApplyingHigherRevision --> Ready
Any --> Empty               (document removed or no valid track)
Any --> Unloaded            (cancel layout and late updates)
```

- `PART_RootGrid`：根层。
- `PART_SingleLinePresenter`：单行字幕。
- `PART_ScrollViewer` 与 `PART_ItemsRepeater`：虚拟化歌词/讲稿。
- `PART_KaraokePresenter`：逐词高亮。
- `PART_PrimaryTextPresenter`、`PART_TranslationTextPresenter`：双语层。
- `PART_EmptyPresenter`、`PART_LiveRegion`：空状态与公告。

除根部件外缺失项只使对应模式降级到 SingleLine。视觉状态组：`DisplayModeStates`、`ContentStates`、`RevisionStates`、`AutoScrollStates`、`CommonStates`。

## 失败模式

- 空文档、无轨道或全无效内容显示空状态，不抛异常。
- 过期 Revision、卸载后的 Provider 更新和已切换文档事件被忽略。
- 翻译缺失、Word 时间不完整或虚拟化项尚未实现时采用确定性降级。
- 控件不记录文本内容、Provider 错误或用户媒体位置。
