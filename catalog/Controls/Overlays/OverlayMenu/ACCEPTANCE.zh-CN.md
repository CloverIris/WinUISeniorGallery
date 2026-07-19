# OverlayMenu Acceptance

## Current gate

验收门禁：当前为 in-progress lab，允许本地实现但尚未稳定。中英文标题结构、稳定 ID 和 API 名称必须同步。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## 自动化场景
Given 10层嵌套 When Invoke/Back Then NavigationPath 与 CurrentItems 严格对应；Given Modal When Esc Then 先返回父级再关闭根菜单；Given NonModal When 叶项未处理 Then 菜单保持打开；Given Disabled item When Invoke Then 不产生 ItemInvoked；Given Scrim/BackButton 缺失 When 操作 Then NonModal/Esc 仍可用。
