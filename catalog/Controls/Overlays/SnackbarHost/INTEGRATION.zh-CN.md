# SnackbarHost 集成

## 注册与生命周期

每个窗口在可覆盖完整客户区的根 Grid 最后一层放置 Host，并以真实 `WindowId` 注册。DI 容器可将 `ISnackbarService` 注册为进程单例，但所有 Show 调用必须携带 `SnackbarHostRegistration`；禁止“最近活动窗口”隐式解析。

服务通过每个 Host 的 `DispatcherQueue` 串行化入队、计时、事件和命令。后台线程可调用 `ShowAsync`，但 Request 必须为不可变快照；结果可在任意上下文 await。Host/Window 销毁或 Registration Dispose 时取消全部任务并解除事件。

## Contract 与平台

依赖 Theme、Motion、Accessibility Contract；`WindowId` 使用 `Microsoft.UI.WindowId`，窗口生命周期通过 `Microsoft.UI.Windowing` 协调。无网络、文件、权限和系统 Toast 能力需求。资源注册在 Controls 主题字典，应用可以覆盖样式但不能更改队列语义。

系统动画关闭时立即显示/关闭；Composition 不可用时使用 XAML Opacity；无 UIA live region 时仍呈现视觉消息并记录诊断事件。应用挂起等同窗口停用，冻结剩余时间而非消耗超时。

## 诊断

Debug 日志仅记录 Request Id、HostName、状态和完成原因，默认不记录 Message 或 Action 参数，避免泄露内容。重复 ID、无效目标和参数错误同步抛出 `ArgumentException`；CancellationToken 已取消则返回已取消 Task 且不入队。
