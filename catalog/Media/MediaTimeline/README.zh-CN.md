# MediaTimeline

## 定位

`MediaTimeline` 是独立于播放器引擎的媒体时间轴，统一呈现点播、纯直播和 Live DVR 窗口，并支持缓冲区间、章节、标记、禁用区间与 Live Edge。

## 范围

- 包含：显示、拖动预览、最终 Seek 请求、键盘步进、章节与区间可视化。
- 不包含：执行 Seek、生成视频缩略图、加载章节元数据或管理播放速度。

`MediaTimelineMath` 提供宿主可复用的区间归一化/合并、禁用区间修正、Live Edge、比例映射和时间格式化纯函数；控件与 Fake Session 使用同一时间域。

## 状态

- 工作项：`in-progress`
- 成熟度：`lab`
- 优先级：`P0`
- 包：`WinUI3.Senior.Media`

## 依赖

- `contracts.media-playback`（`MediaPlaybackTimeRange` 由 Core 拥有）

## 文档

- [规范](SPEC.zh-CN.md)
- [设计](DESIGN.zh-CN.md)
- [集成](INTEGRATION.zh-CN.md)
- [验收](ACCEPTANCE.zh-CN.md)

## Agent 所有权

实现 agent 只能修改 `feature.json` 的 `owned_paths`。时间域或播放契约不足时标记 `blocked`，不得自行创造第二套媒体时间类型。
