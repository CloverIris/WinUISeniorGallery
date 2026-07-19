# FileCard Acceptance

## Current gate

验收门禁：当前为 in-progress lab，允许本地实现但尚未稳定；中英文标题、稳定 ID 和 API 名称必须同步。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given 合成 Descriptor When 页面加载 Then 不打开文件；Given Thumbnail null When 渲染 Then 元数据和动作仍可用；Given PreviewRequested When 宿主 Handled=false Then 控件不自行读取；Given disabled action When InvokeAction Then 无事件；Given Light/Dark/High Contrast/RTL/200% DPI Then 名称、SizeText 和动作可读可达。
