# BottomSheet 设计

## 视觉与动效

Bottom 模式顶部圆角，Side 模式靠窗口边缘侧为直角，Center 四角圆角。Surface 使用系统 Layer Fill 和主题阴影；模态 Scrim 默认 32% 黑色并通过 ThemeResource 覆盖。拖动把手最小视觉 32×4px、命中区域 44×24px。

打开与吸附使用 Composition 位移，目标 280ms；关闭 220ms。速度影响弹簧初速度但不得越过目标吸附点。Reduced Motion 时取消位移弹簧，使用不超过 100ms 淡入淡出，并立即完成吸附。

## 响应式、主题和资源

公开资源键：`BottomSheetSurfaceBackground`、`BottomSheetScrimBrush`、`BottomSheetCornerRadius`、`BottomSheetShadow`、`BottomSheetDragHandleBrush`、`BottomSheetOpenDuration`、`BottomSheetCloseDuration`、`BottomSheetMinExtent`。Light/Dark/Mica/Acrylic 宿主中仅使用系统资源；High Contrast 禁用阴影和透明 Scrim，改用系统 Window/Text 颜色和 2px 边框。

软键盘、标题栏 inset、任务栏和显示裁剪均计入可用区域。内容超出最大范围时由模板内部 ScrollViewer 滚动，Sheet 本身不越出窗口。

## 无障碍

模态 AutomationPeer 暴露 Window/Transform 模式并报告 `IsModal=true`；非模态为 Pane。SnapPoint 改变公告可读名称；拖动中的连续像素变化不公告。把手支持 Enter/Space 在吸附点间循环、上下方向键移动、Esc 关闭。
