# MediaPlayback

## 定位

`MediaPlayback` 是 Core 对媒体引擎的唯一稳定会话抽象。它定义不可变快照、时间域、能力和可取消命令，Media 控件只能依赖这些类型。

## 状态

`in-progress / lab / P0`。已实现的契约仍须与 Media 控件和适配器完成集成验证，方可进入 review。

## 范围

包含 `IMediaPlaybackSession`、`MediaPlaybackSnapshot`、`MediaPlaybackTimeRange`、命令结果和修订事件；不包含任何播放器、XAML 控件、媒体源、文件路径、窗口或网络服务。

## 文档

- [规范](SPEC.zh-CN.md)
- [设计](DESIGN.zh-CN.md)
- [集成](INTEGRATION.zh-CN.md)
- [验收](ACCEPTANCE.zh-CN.md)

## 所有权

本工作单元独占 `contracts/MediaPlayback`、Core 的 `MediaPlayback` 源码和对应 Core 测试。功能控件不得复制公共模型。
