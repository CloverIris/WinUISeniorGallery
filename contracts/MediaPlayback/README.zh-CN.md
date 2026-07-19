# Media Playback Contract

## 契约身份

- Contract ID: `contract.media-playback.v1`
- Dependencies: contract.architecture.v1
- 规范性关键词 MUST、MUST NOT、SHOULD 与 MAY 按其强度具有约束力。
- 中文 zh-CN 是规范源；en-US 必须同步标题层级、ID、API、资源键和事件名。

## 适用范围

定义播放器无关会话、播放状态、命令、时间轴、Live DVR 与宿主请求。
- 适用于所有消费本契约的生产控件、体验、Gallery 集成、适配器和测试。
- 本契约不把功能专属公共 API 的所有权转移给契约包。

## 术语

- `PlaybackSessionId`: 稳定会话身份.
- `MediaTime`: 单调媒体时钟.
- `SeekPreview`: 未提交预览位置.
- `HostRequest`: 全屏.

## 责任方

- 契约所有者 MUST 维护稳定语义、兼容性说明和一致性测试。
- Provider MUST 验证输入、遵守取消、发布规定结果并释放作用域资源。
- Consumer MUST 依赖抽象，不得检查 Provider 的实现类型。
- 除非用户显式配置外部能力，Gallery MUST 使用本地确定性数据。
- 功能所有者 MUST 记录偏差；不兼容的契约缺口解决前保持 blocked。

## 规范性规则

- UI 不引用具体播放器；适配器可连接 MediaPlayerElement、C++ 或远端引擎。
- 快照带单调递增 Revision，旧修订被忽略。
- Seek Preview 与 Commit 分离，释放时只提交一次。
- Live DVR 起止时间可移动，位置始终相对同一 MediaTime。
- 全屏/CompactOverlay/浮窗只发布 HostRequest，不创建窗口。
- P0 Media 不依赖 Windowing。

- 每个外部可见状态 MUST 具有确定性降级。
- 每个异步操作 MUST 支持取消，或证明完成时间有界。
- 诊断 MUST 使用稳定代码，并且 MUST NOT 包含用户内容。

## 最小契约面

实现所有权、精确签名和验收场景位于 [Foundation MediaPlayback](../../catalog/Foundation/MediaPlayback/README.zh-CN.md)。本文件定义跨模块必须遵守的语义；Core 是唯一代码所有者。

### 接口与模型

- `IMediaPlaybackSession`: `CurrentSnapshot`、`SnapshotChanged` 以及六个可取消命令 `PlayAsync`、`PauseAsync`、`StopAsync`、`SeekAsync`、`SetVolumeAsync`、`SetPlaybackRateAsync`。
- `MediaPlaybackSnapshot`: 不可变递增修订快照，包含会话身份、状态、模式、能力、媒体时间、区间、音量、倍速及稳定错误码。
- `MediaPlaybackTimeRange`: 所有 Consumer 共用的闭合媒体时间区间；Timeline 不得复制第二种公共区间类型。
- `MediaPlaybackCommandResult`: `Success`、`Rejected`、`Cancelled` 或 `Failed` 的可观察命令结果。

- 公共值 MUST 是不可变快照或有明确文档的命令。
- 命令 MUST 返回可观察结果：success、rejected、cancelled 或 failed。
- 稳定身份 MUST 按值比较，并且 MUST NOT 依赖对象引用身份。

### 资源与事件

- 公共资源键 MUST 遵循 Resources Contract 且只有一个所有者。
- 事件可能过期投递时 MUST 携带单调递增 revision 或 generation。
- 事件投递 MUST 在规定作用域内保持有序。
- 订阅 MUST 可释放，并且 MUST NOT 阻止窗口销毁。
- 事件名、API 名、稳定 ID 和资源键在所有区域中保持英文。

## 线程与窗口范围

适配器在后台接收原生事件并合并；快照与命令结果在会话所属 UI Dispatcher 有序发布。
- UI 对象 MUST 只在所属 Dispatcher 上创建、读取和修改。
- 后台工作 MAY 处理不可变数据，发布前 MUST 检查取消。
- 已关闭窗口 MUST NOT 接收 Navigation、Snackbar、Automation 或视觉更新。
- 跨窗口消息 MUST 只包含稳定 ID 与不可变数据，不得包含 UIElement、XamlRoot 或路由事件参数。
- latest-wins Consumer MUST 拒绝过期 revision/generation。

## 错误与降级

已释放会话命令返回 cancelled；不支持命令返回 rejected；引擎错误提供稳定 code/correlation，不抛跨窗口异常。
- rejected 表示请求有效，但当前策略或能力不支持。
- 除非取消清理失败，cancelled 不记录为运行故障。
- failed 结果 MUST 包含稳定错误码；correlation ID MUST 不含内容。
- 降级 MUST 保留核心可读性和键盘操作。
- 重复等价错误 SHOULD 合并，避免诊断和 LiveRegion 轰炸。

## 安全与隐私

- 来自 route、Provider、清单和持久设置的输入在验证前均不可信。
- 凭据、个人内容、文件路径、媒体、字幕和联系人 MUST NOT 进入默认遥测。
- 外部链接与高权限平台操作需要用户显式激活。
- 资产打包前 MUST 通过许可审查。
- Gallery 演示默认使用本地 fake 数据。

## 版本与兼容

可新增可选能力；改变命令、时间单位、修订或 HostRequest 语义需要 v2。
- 向后兼容新增时 Contract ID 保持稳定。
- 新增可选字段 MUST 为旧 Consumer 提供确定性默认值。
- 弃用 MUST 明确替代项、迁移方式和最短支持窗口。
- v1 Provider MUST 拒绝未知必需行为，不得静默近似。
- Package 版本与 Contract 版本相关但不可互换。

## 测试与验收矩阵

- VOD: MUST have deterministic automated evidence.
- Seek Preview: MUST have deterministic automated evidence.
- 乱序原生事件: MUST have deterministic automated evidence.
- 释放: MUST have deterministic automated evidence.
- success、rejected、cancelled、failed 和降级路径 MUST 全部覆盖。
- 涉及 UI 时，测试 MUST 覆盖 Light、Dark、High Contrast、RTL、Reduced Motion、键盘和屏幕阅读器。
- 测试 MUST 覆盖窗口关闭和过期异步投递。
- 性能测试 MUST 使用本地确定性数据报告 median 与 P95。
- 一致性测试不得依赖网络、登录或未许可历史资产。

## 交付门槛

- Provider 与至少一个 Consumer 必须能针对契约编译，且不泄漏实现类型。
- 架构校验不得发现反向依赖、重复所有者、循环或所有权重叠。
- 自动化测试必须证明规定的降级与清理行为。
- 中英文标题深度序列、ID、API 和资源键必须一致。
- 可访问性与隐私审查问题必须解决或明确标为 blocking。
- 破坏性歧义在契约所有者解决前阻止状态推进。

## 禁止模式

- 进程级可变 UI 状态。
- 没有有界生命周期与诊断的 fire-and-forget 工作。
- 目标窗口关闭后静默改投其他窗口。
- 硬编码主题颜色或拼接面向用户的自然语言。
- 在稳定公共模型中暴露供应商 SDK 类型。
- 把 Gallery 模拟当作 Windows 系统能力。
