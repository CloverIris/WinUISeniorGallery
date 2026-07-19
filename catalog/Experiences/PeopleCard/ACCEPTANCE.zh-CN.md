# PeopleCard Acceptance

## Current gate

验收门禁：当前为 in-progress lab，允许本地实现但尚未稳定；中英文标题、稳定 ID 和 API 名称必须同步。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given 合成 Person When 页面加载 Then 只显示宿主字段；Given disabled action When InvokeAction Then 无事件；Given ToggleExpanded When 操作 Then 状态改变且数据不丢失；Given unload When action arrives Then 不访问窗口/通讯录；Given High Contrast/RTL/200% DPI Then 主要字段和动作可达。
