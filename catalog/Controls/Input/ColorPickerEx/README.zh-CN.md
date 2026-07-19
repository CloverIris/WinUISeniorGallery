# ColorPickerEx

这是中文规范源。当前为 in-progress 实现工作单元；实现仍需后续模板、视觉验收与测试收尾。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Input/ColorPickerEx

## 实现准备
当前实现包含 Hex/RGB 文本解析、RGB/HSV/HSL 通道转换、调色板与有界历史、可选 `IColorEyedropperProvider` 和校验事件。屏幕取色宿主、模板、视觉验收和自动化测试仍待收尾。
