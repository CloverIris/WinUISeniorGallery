# DetachablePlayerHost Acceptance

## Current gate

当前为 `in-progress`；Fake Host 可用于人工观察，仍需后续视觉和运行时验证后进入 `review`。

## Given / When / Then

- Given 已绑定宿主且处于 Inline，When 调用 `DetachAsync`，Then 发送一次 Open 请求并进入 Detached。
- Given Detached，When 调用 `AttachAsync`，Then 发送一次 Close 请求并回到 Inline。
- Given 操作正在等待，When owner close 或 host 被替换，Then 迟到结果被取消且不覆盖新状态。
- Given 未绑定宿主、非法尺寸或错误状态，When 调用操作，Then 返回稳定 Rejected 错误码。

## Matrix

Light/Dark/High Contrast、Reduced Motion、RTL、100%/150%/200% DPI、键盘按钮、窗口销毁和取消。
