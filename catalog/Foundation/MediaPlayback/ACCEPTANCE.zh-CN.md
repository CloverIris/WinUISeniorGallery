# MediaPlayback 验收

## 契约场景

- Given 空白或已释放会话，When 调用命令，Then 返回 `Cancelled/Disposed` 且不抛跨线程异常。
- Given 不支持的命令，When Consumer 请求它，Then 返回 `Rejected/Unsupported`。
- Given 快照 Revision 为 4 后收到 Revision 为 3，When Consumer 合并状态，Then 保留 Revision 4。
- Given 可变输入缓冲集合，When 创建快照后修改输入，Then 快照内容不变。

## 自动化与质量门槛

Core 单元测试覆盖值验证、范围钳制、不可变复制和四种命令结果。Media Adapter 集成测试覆盖 Dispatcher 顺序、释放和晚到事件。通过 Core Release x64、架构校验及至少一个 Media Consumer 编译后才可进入 review。

## 实现证据

- 2026-07-16：Core Release x64 测试通过 4/4，Gallery Debug/Release x64 构建成功，架构测试通过 4/4。
- 真实本地媒体的 Adapter 手工验证仍在 Gallery review 阶段；当前保持 `in-progress`。
