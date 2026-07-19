# WidgetCard

这是中文规范源。当前为 in-progress lab，已具备宿主刷新 Provider、自动刷新暂停和错误恢复逻辑。

## Status

in-progress / lab / P1

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Layout/WidgetCard

## 实现准备
可折叠、可调整尺寸的 Dashboard 卡片；后台刷新属于宿主服务。进入 ready 前锁定可用尺寸枚举。
