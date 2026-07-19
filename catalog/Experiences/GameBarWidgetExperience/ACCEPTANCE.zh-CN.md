# GameBarWidgetExperience Acceptance

## Current gate

当前为 `in-progress`；本地 Fake Host 已可观察，仍需视觉和运行时验证。

## Given / When / Then

- Given 已绑定宿主，When Open，Then 进入 Interactive 并发送一次 host 请求。
- Given Interactive，When 宿主确认 ClickThrough，Then 进入 ClickThrough 且保留恢复热键。
- Given 未确认或恢复热键为空，When 请求 ClickThrough，Then 返回 false 且保持原模式。
- Given ClickThrough，When Minimize/Restore，Then 内容不销毁并恢复 ClickThrough。
