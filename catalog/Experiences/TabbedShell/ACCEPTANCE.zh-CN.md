# TabbedShell Acceptance

## Current gate

验收门禁：proposed 工作单元不得实现。中英文标题结构、稳定 ID 和 API 名称必须同步；API、状态、错误和性能预算锁定后才可进入 ready。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given 创建/保存/关闭取消竞争 Then Tab恰有一个WindowId且Dirty不丢；100标签虚拟化，恢复p95≤1s。
