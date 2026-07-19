# OverlayMenu

这是中文规范源。当前为 in-progress lab，已具备层级导航、模态遮罩和 Esc/Back 语义实现。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Overlays/OverlayMenu

## 实现准备
层级覆盖菜单默认 Modal、Right placement；Esc 优先返回父级，再关闭根菜单；非模态菜单选择项不会自动关闭。宿主拥有菜单数据和动作。
