# EditorCanvas Acceptance

## Current gate

验收门禁：当前为 in-progress lab，允许本地实验，不代表稳定 API。中英文标题结构、稳定 ID 和 API 名称必须同步；进入 ready 前补齐 100k 对象性能和持久化决策。

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given 空文档 When 页面加载 Then 画布可显示且不访问文件；Given 合成对象 When Select + 拖拽释放 Then 选区移动并 Revision +1；Given Rectangle/Text When 拖拽释放 Then 插入对象并自动选中；Given Ink When pen/touch 样本到达并释放 Then 生成一个含压力数据的 CanvasStroke；Given Ink When Esc/取消 Then 不插入对象；Given Snap When 移动或箭头导航 Then 坐标按 GridSize 对齐；Given Delete/Undo/Redo When 操作 Then 只影响 Core 快照；Given Dark/High Contrast/RTL/Reduced Motion When 切换 Then 画布仍可操作。后续 ready 门禁：100k 对象虚拟化/渲染预算必须用可观测指标记录。
