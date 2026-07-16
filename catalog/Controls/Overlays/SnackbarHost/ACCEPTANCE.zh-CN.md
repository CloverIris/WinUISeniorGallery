# SnackbarHost 验收

## 功能场景

- Given 两个窗口各自注册 Host，When 向 A 入队三条并销毁 A，Then A 全部返回 HostDestroyed，B 队列和视觉完全不变。
- Given Normal-1、Low-1、High-1 等待，When 当前项结束，Then 顺序为 High-1、Normal-1、Low-1；当前项从不被抢占。
- Given 相同 Key 的待显示项，When 新项到达，Then 原任务 Replaced，新项保持原队列位置；Given 当前项，Then 原地更新且只公告一次。
- Given 当前项剩余 2s，When 指针悬停 5s，Then 离开后仍约 2s；窗口停用、焦点进入和 IsPaused 行为相同。
- Given Action 与超时同帧竞争，Then 仅一个完成原因，命令至多执行一次。
- Given Registration Dispose 或 Host 卸载，Then 无动画清理计时器、队列、事件和 Composition 对象，消息不迁移。

## 质量矩阵

- 验证鼠标、触摸、键盘、Narrator；Light/Dark/High Contrast、RTL、Reduced Motion、100/200% DPI 和 320/800/1920px 宽度。
- UIA 验证普通消息 polite、Warning/Error/Critical assertive、视觉截断但 Automation 全文、焦点不被自动抢占。
- 并发测试从 8 个线程向同一/不同 Host 各提交 1000 条，结果无丢失、重复完成或跨窗口显示；每 Host 顺序满足稳定优先级。
- 稳态空 Host 无活动计时器；持续 10,000 次入队完成后 Registration、Request、命令参数无保留泄漏。
- 单元测试覆盖参数、Duration 边界、重复 ID、去重、取消竞争；UI 测试覆盖动画状态、暂停剩余时间、Action/Dismiss 命中区与销毁。
