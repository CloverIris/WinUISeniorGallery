# WidgetsBoard Acceptance

## Current gate

验收门禁：proposed 工作单元不得实现。中英文标题结构、稳定 ID 和 API 名称必须同步；API、状态、错误和性能预算锁定后才可进入 ready。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given 损坏/旧schema Then 迁移或隔离单Widget；100Widgets拖动p95≤25ms，保存失败不丢当前布局并可重试。
