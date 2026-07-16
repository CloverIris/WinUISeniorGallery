# SnackbarHost 规格

## 目标与非目标

为每个 WinUI 窗口维护严格隔离、优先级稳定的单消息队列，支持超时、操作按钮、程序取消、CancellationToken、去重及屏幕阅读器公告。不实现系统 Toast、跨进程通知、通知历史或后台任务唤醒。

## 公共 API

```csharp
public enum SnackbarPriority { Low, Normal, High, Critical }
public enum SnackbarVisualKind { Informational, Success, Warning, Error }
public enum SnackbarCompletionReason { TimedOut, ActionInvoked, Dismissed, Cancelled, HostDestroyed, Replaced }
public enum SnackbarPlacement { BottomLeft, BottomCenter, BottomRight, TopLeft, TopCenter, TopRight }

public sealed record SnackbarAction(string Label, ICommand Command, object? CommandParameter = null);
public sealed record SnackbarRequest(
    Guid Id,
    string Message,
    SnackbarVisualKind Kind = Informational,
    SnackbarPriority Priority = Normal,
    TimeSpan? Duration = null,
    SnackbarAction? Action = null,
    string? DeduplicationKey = null,
    bool IsDismissible = true,
    string? AutomationAnnouncement = null);
public sealed record SnackbarResult(Guid RequestId, SnackbarCompletionReason Reason);

public sealed class SnackbarHostRegistration : IDisposable
{
    public WindowId WindowId { get; }
    public string? HostName { get; }
    public bool IsValid { get; }
    public void Dispose();
}

public sealed class SnackbarHost : Control
{
    public string? HostName { get; set; }
    public SnackbarPlacement Placement { get; set; } // BottomCenter
    public bool IsPaused { get; set; }
    public event EventHandler<SnackbarShownEventArgs> Shown;
    public event EventHandler<SnackbarClosedEventArgs> Closed;
}

public interface ISnackbarService
{
    SnackbarHostRegistration Register(WindowId windowId, SnackbarHost host, string? hostName = null);
    Task<SnackbarResult> ShowAsync(SnackbarHostRegistration target, SnackbarRequest request, CancellationToken cancellationToken = default);
    bool Cancel(SnackbarHostRegistration target, Guid requestId);
}
```

`SnackbarHostRegistration` 是不可伪造、可释放的目标令牌，公开 `WindowId`、`HostName`、`IsValid`；释放后调用 `ShowAsync` 返回 `HostDestroyed`。同一 Host 只能注册一次；同窗口允许命名不同 Host，`(WindowId, HostName)` 必须唯一。

`Id` 不得为空且在目标未完成集合中唯一；`Message` 非空；Action Label 非空。显式 Duration 范围 `[1s, 60s]`。默认 Duration：Low/Normal 5s、High 8s、Critical 10s；系统无障碍“延长通知”设置按全局 Contract 放大但不超过 60s。

## 队列、优先级与去重

每个 Host 同时只显示一条。当前消息不被高优先级抢占；待显示队列按 Priority 降序、同优先级 FIFO。当前消息完成后取最高优先级最早项。

`DeduplicationKey` 为空时不去重。相同非空 Key：

1. 命中待显示项：新请求替换原位置内容但保持原队列序号；旧任务以 `Replaced` 完成。
2. 命中当前项：原项立即以 `Replaced` 完成，视觉原地更新为新项并重新计时，不执行关闭/打开动画。
3. Key 比较为 Ordinal；只在同一个 Host 内生效。

`Cancel` 或 CancellationToken 命中待显示项时移除并返回 `Cancelled`；命中当前项时播放关闭动画后返回 `Cancelled`。完成竞争采用第一个原子提交者，后续操作无效。Action 先原子完成为 `ActionInvoked`，再在 UI 线程执行命令；命令异常通过 DispatcherQueue 未处理异常路径报告，但不改变完成结果。

## 状态、模板和行为

Host 状态为 `Empty`、`Showing`、`Visible`、`Closing`、`Destroyed`。窗口停用、指针悬停、键盘焦点在 Snackbar 内或 `IsPaused=true` 时冻结剩余超时；恢复后使用剩余时长。窗口销毁不播放动画，当前及排队任务均以 `HostDestroyed` 完成，禁止转发到其他 Host。

模板部件：`PART_Root`、`PART_Surface`、`PART_MessagePresenter`、可选 `PART_IconPresenter`、`PART_ActionButton`、`PART_DismissButton`、`PART_LiveRegion`。VisualStateGroups：`DisplayStates`、`KindStates`、`InteractionStates`、`PlacementStates`。缺少 Surface 或 MessagePresenter 抛模板异常；可选部件缺失时功能安全隐藏。

Esc 仅在 Snackbar 内有焦点且 `IsDismissible` 时关闭；Dismiss 按钮同理。Action 只能执行一次。空闲 Host 不拦截命中测试。
