# MediaPlayback 规范

## 目标与非目标

目标是在不泄漏供应商类型的前提下表达播放状态与命令。非目标是加载媒体、选择文件、创建窗口、处理系统媒体控制或提供 Presenter 请求。

## 公共 API

`IMediaPlaybackSession` 提供 `CurrentSnapshot`、`SnapshotChanged` 及 `PlayAsync`、`PauseAsync`、`StopAsync`、`SeekAsync`、`SetVolumeAsync`、`SetPlaybackRateAsync`。所有命令接受 `CancellationToken` 并返回 `MediaPlaybackCommandResult` 的 `Success`、`Rejected`、`Cancelled` 或 `Failed`。`MediaPlaybackSnapshot` 的 `Revision` 对同一会话单调递增，包含 `PlaybackSessionId`、状态、模式、能力、位置、可 Seek 区间、缓冲区间、音量、倍速和稳定错误信息。

## 状态与数据

状态为 `Unavailable`、`Loading`、`Ready`、`Playing`、`Paused`、`Buffering`、`Ended`、`Failed`。`MediaPlaybackTimeRange` 是闭区间；无效区间不可用于位置钳制。快照和缓冲集合在构造后不可变，Consumer 必须忽略过期 Revision。

## 行为与失败

已释放会话返回 `Cancelled/Disposed`；不支持的有效命令返回 `Rejected/Unsupported`；无效参数返回 `Rejected/InvalidArgument`；引擎故障返回 `Failed/EngineFailure`，不得把原始内容或文件路径写入错误。事件按会话 UI Dispatcher 顺序投递。
