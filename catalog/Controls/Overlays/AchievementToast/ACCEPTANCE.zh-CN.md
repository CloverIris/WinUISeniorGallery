# AchievementToast Acceptance

## Current gate

验收门禁：当前为 in-progress lab，允许本地实现但尚未稳定。中英文标题结构、稳定 ID 和 API 名称必须同步。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## 自动化场景
Given 两个窗口 When 分别 ShowAsync Then 队列互不影响；Given 32+请求 When 入队 Then 超限返回 queue-full；Given 当前项 When 超时/Dismiss Then 只完成一次并展示下一项；Given Dispose When 队列存在 Then 所有等待项返回 HostDestroyed；Given Light/Dark/High Contrast/Reduced Motion When 切换 Then 内容可读且无计时器泄漏。
