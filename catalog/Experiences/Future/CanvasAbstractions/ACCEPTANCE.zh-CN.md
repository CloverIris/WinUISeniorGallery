# Canvas.Abstractions Acceptance

## Current gate

验收门禁：proposed 工作单元不得实现。中英文标题结构、稳定 ID 和 API 名称必须同步；API、状态、错误和性能预算锁定后才可进入 ready。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then 与 Ready 门禁
Given100k对象/10k点笔划/撤销重放 Then结果确定、内存有界。Ready需ABI fuzz、崩溃恢复、线程和序列化兼容测试。
