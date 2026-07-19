# MediaPlayerChrome

## 定位

`MediaPlayerChrome` 是与播放引擎解耦的现代媒体控制层。它消费 `IMediaPlaybackSession`，统一呈现传输控制、时间轴、音量、倍速、全屏和浮窗请求。

## 范围

- 包含：控制层 UI、状态映射、命令转发、自动隐藏、错误与缓冲反馈。
- 不包含：解码、媒体源加载、窗口创建、内容跨窗口迁移、系统媒体传输控制注册。
- P0 提供 `MediaPlayerElementPlaybackSessionAdapter`，但控件不得依赖 `MediaPlayerElement` 的具体类型。

## 状态

- 工作项：`in-progress`
- 成熟度：`lab`
- 优先级：`P0`
- 包：`WinUI3.Senior.Media`

## 依赖

- `foundation.media-playback`
- `media.media-timeline`

## 文档

- [规范](SPEC.zh-CN.md)
- [设计](DESIGN.zh-CN.md)
- [集成](INTEGRATION.zh-CN.md)
- [验收](ACCEPTANCE.zh-CN.md)

## Agent 所有权

实现 agent 只能修改 `feature.json` 的 `owned_paths`。公共播放契约不足时应把本工作项标为 `blocked`，不得在功能目录内复制接口。
