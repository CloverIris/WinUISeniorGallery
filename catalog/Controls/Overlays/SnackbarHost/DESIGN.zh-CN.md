# SnackbarHost 设计

## 视觉、布局与动效

默认 BottomCenter，距离客户区安全边缘 24px，最大宽度 480px、最小宽度 288px；小于 336px 时左右各留 16px。内容为图标、可换行消息、单个文本 Action 和可选关闭按钮。消息最多视觉显示 3 行，但 Automation Name 保留全文。

显示使用 16px 位移加淡入 180ms，关闭 120ms；同 Key 原地替换使用 100ms 交叉淡入。Reduced Motion 仅保留不超过 100ms 淡入；High Contrast 不使用阴影或半透明背景。

## 主题资源

公开资源键：`SnackbarHostMaxWidth`、`SnackbarHostMargin`、`SnackbarSurfaceBackground`、`SnackbarForeground`、`SnackbarBorderBrush`、`SnackbarCornerRadius`、`SnackbarShadow`、`SnackbarActionButtonStyle`、`SnackbarShowDuration`、`SnackbarCloseDuration`，以及四种 Kind 的图标/强调色键。全部通过 ThemeResource，Error 不得只靠颜色表达。

## 输入与无障碍

Snackbar 出现时默认不窃取焦点；Action 可通过 Tab 到达。普通消息使用 polite live region，Warning/Error/Critical 使用 assertive；`AutomationAnnouncement` 非空时作为公告文本，否则使用 Message。去重原地更新只公告新文本一次。

触摸、鼠标和键盘具有不小于 44×44px 的 Action/Dismiss 命中区。RTL 镜像图标、操作和关闭顺序。屏幕阅读器或键盘焦点位于消息内时暂停超时。
