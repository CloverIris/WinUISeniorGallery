# DetachablePlayerHost Acceptance

## Current gate

验收门禁：proposed 工作单元不得实现。中英文标题结构、稳定 ID 和 API 名称必须同步；API、状态、错误和性能预算锁定后才可进入 ready。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given 创建窗口失败 Then 原位继续播放并可重试；Given 任一窗口关闭矩阵 Then 会话恰好一个所有者；迁移p95≤250ms且无音频中断。
