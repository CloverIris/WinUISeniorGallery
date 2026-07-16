# CarouselView 设计

## 信息层级与布局

`PART_Root` 承载一个裁剪的主内容区、可选前后导航按钮、可选指示器和不可见的 Live Region。选中项是视觉主层；仅在 `IsAdjacentPreviewEnabled=true` 时才显示相邻实现项。窄窗口优先保证主项触控目标，常规和宽窗口可显示相邻预览，但预览不能遮挡标题、焦点框或按钮。

## 视觉与主题

控件必须只使用 `CarouselViewBackground`、`CarouselViewNavigationButtonStyle`、`CarouselViewIndicatorForeground`、`CarouselViewIndicatorSelectedForeground`、`CarouselViewItemSpacing`、`CarouselViewTransitionDuration`、`CarouselViewCornerRadius` 主题资源。Light、Dark、High Contrast 和主题运行时切换均不得重建数据源。高对比度下以系统前景/背景和可见焦点为准，不依赖透明度表达状态。

## 交互与动效

按钮、键盘、滚轮、拖拽和手柄最终提交相同的选择变更。拖拽进入 `Dragging`，完成后进入 `Settling` 或 `Idle`；取消、卸载和集合变更必须安全结束手势。`Slide`、`Fade`、`CoverFlow` 由 Composition 增强，不能影响选择语义。Reduced Motion 或 Composition 不可用时转场不超过 100ms，或者直接收敛。

## 输入、焦点与无障碍

LTR/RTL 的左右方向镜像；Home/End 保持逻辑首末项。普通垂直滚轮不截获。焦点顺序为 Previous（若存在）、内容、Next（若存在）、指示器（若存在）；焦点进入/离开按配置暂停或恢复自动播放。Automation 提供 Selection、Scroll 和 ItemContainer；Live Region 只公告用户动作。触控板无法可靠识别时设备类型为 `Unknown`。

## 响应式与现代化取舍

必须在 320、800、1920 有效像素宽度及 100%、150%、200% DPI 下保持完整可操作，文本缩放时不裁切交互目标。中文、英文与 RTL 不得假定固定文本宽度或 LTR 几何。该设计借鉴横向媒体浏览体验，但不复制任何历史产品视觉资产。
