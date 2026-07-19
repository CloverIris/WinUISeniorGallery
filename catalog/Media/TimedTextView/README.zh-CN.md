# TimedTextView

## 定位

`TimedTextView` 是统一的时间文本显示控件，以 Document → Track → Segment → Word 模型覆盖单行字幕、滚动歌词、Karaoke 逐词高亮、双语文本和实时 ASR 修订。

## 范围

- 包含：时间匹配、增量快照、轨道选择、虚拟化呈现、自动滚动、原文与翻译显示。
- 不包含：语音识别、机器翻译、字幕文件解析、媒体播放或 Provider 凭据管理。

## 共享时间投影

`TimedTextProjectionCalculator` 将当前位置投影为不可变的 `TimedTextProjection`：包括有效时间、活动 Segment/Word、0–1 段落与逐词进度、翻译回退和上下文窗口。控件和未来的 ASR/翻译适配器共用这套半开区间规则，避免各页面自行实现时间匹配。

控件还提供显式轨道切换、自动滚动暂停/恢复和 `ActiveTrackChanged` 事件；用户滚动或离开宿主时不会偷偷修改播放会话。

## 状态

- 工作项：`in-progress`
- 成熟度：`lab`
- 优先级：`P0`
- 包：`WinUI3.Senior.Media`

## 依赖

- `foundation.timed-text`
- `foundation.media-playback`

## 文档

- [规范](SPEC.zh-CN.md)
- [设计](DESIGN.zh-CN.md)
- [集成](INTEGRATION.zh-CN.md)
- [验收](ACCEPTANCE.zh-CN.md)

## Agent 所有权

实现 agent 只能修改 `feature.json` 的 `owned_paths`。Provider 只通过全局抽象接入，禁止在控件中添加具体云服务依赖。
