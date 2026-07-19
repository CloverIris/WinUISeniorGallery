# CaptionsTranslationExperience Specification

## Goal

为 `TimedTextView` 提供宿主字幕 Revision 合并层，保证 Revision 单调、翻译轨回退和错误降级。

## Non-goals

不识别语音、不翻译、不解析字幕文件、不联网、不创建 Provider 实例。

## Public API

`CaptionsTranslationRevision(Revision, Source, Translation, IsFinal, ErrorCode)`；`State`（Idle/Listening/Translating/Degraded/Error）、`DisplayMode`、`IsAutoFollowEnabled`、`ApplyRevision`、`SetPosition`、`SetError`。Revision 小于等于当前值必须拒绝且不改变呈现。

## Projection

必须部件 `PART_TimedTextView` 和 `PART_Status`。Source 与 Translation 的轨合并为不可变文档；重复轨 ID 保留首个。错误 Revision 显示降级状态，但仍可呈现最后一个可接受文档。卸载/Provider 生命周期由宿主控制。
