# EditorCanvas Design

## Visuals and interaction

画布区域是页面主视觉，工具栏提供 Select/Pan/Rectangle/Text、Fit、Undo/Redo、Grid 和 Snap。对象使用圆角卡片表达，不复刻 Office 截图或资产；选中对象应有可见焦点/对比度边界，拖拽中的预览与释放后的 Core Revision 分离。

## Responsiveness

窄窗口将工具栏换行并保持画布最小高度；常规窗口显示完整工具栏；宽窗口允许画布扩展。滚轮缩放围绕指针锚点，平移不改变世界坐标方向。

## Theme, motion, input, and accessibility

Light/Dark 使用 ThemeResource；High Contrast 保留对象与网格对比；Reduced Motion 不使用惯性或过渡。键盘箭头每次移动 1 世界单位，Snap 开启时按 GridSize 对齐。Automation Name 以 `Canvas object {id}` 暴露。

## Modernization tradeoffs

现代化取舍：优先保留可预测的文档状态和宿主边界，不把保存、协作、文件格式或 C++ 压感能力塞进控件；历史来源由 Archaeology 工作单元维护。

## 响应式、输入与无障碍
鼠标/触摸/笔/键盘、压感与框选需等价命令；RTL影响UI不反转画布坐标，高对比度参考线可见，Reduced Motion无惯性。
