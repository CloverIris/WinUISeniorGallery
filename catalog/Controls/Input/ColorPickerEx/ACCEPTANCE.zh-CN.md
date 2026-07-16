# ColorPickerEx Acceptance

## Current gate

验收门禁：proposed 工作单元不得实现。中英文标题结构、稳定 ID 和 API 名称必须同步；API、状态、错误和性能预算锁定后才可进入 ready。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## 自动化场景
覆盖HEX/RGB/HSV往返、alpha、非法文本、吸管取消、键盘/Narrator；8-bit往返误差≤1且事件一次。
