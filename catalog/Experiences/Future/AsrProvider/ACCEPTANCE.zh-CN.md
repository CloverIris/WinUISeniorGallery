# ASR Provider Acceptance

## Current gate

验收门禁：proposed 工作单元不得实现。中英文标题结构、稳定 ID 和 API 名称必须同步；API、状态、错误和性能预算锁定后才可进入 ready。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then 与 Ready 门禁
Given撤权/离线/限流/语言不支持 Then 映射标准错误且不泄露音频。Ready需本地+远程假Provider、取消延迟≤250ms及威胁模型。
