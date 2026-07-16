# Canvas.WinUI Acceptance

## Current gate

验收门禁：proposed 工作单元不得实现。中英文标题结构、稳定 ID 和 API 名称必须同步；API、状态、错误和性能预算锁定后才可进入 ready。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then 与 Ready 门禁
Given依赖未Ready Then构建不含运行时类型；未来设备丢失恢复焦点/内容，8ms输入延迟目标、p95帧≤16.7ms。Ready需UIA/多DPI/泄漏矩阵。
