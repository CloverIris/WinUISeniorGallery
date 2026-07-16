# HomeScreen Acceptance

## Current gate

验收门禁：proposed 工作单元不得实现。中英文标题结构、稳定 ID 和 API 名称必须同步；API、状态、错误和性能预算锁定后才可进入 ready。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given 局部超时 When 其余完成 Then 仅该节显示重试；10k项只实现视口，首屏p95≤500ms，返回页面恢复焦点/滚动。
