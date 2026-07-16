# TitleBarHost 设计

## 视觉层级与响应式

候选树：RootGrid、IconPresenter、TextStack、Left/Center/RightPresenter、DragSurface、CaptionInsetSpacer。交互内容自动从拖动区域排除。 窄/常规/宽和 100%–300% DPI 共用状态模型；窗口改变不重建宿主数据，200% 文本缩放优先重排/溢出。

## 输入与焦点

Tab 只进入交互内容，不进入拖动背景；系统按钮顺序、RTL、Alt+Space 和双击标题栏均保留。

## 主题、动效与无障碍

仅用 ThemeResource；High Contrast 不依赖材质/透明度。Reduced Motion 采用即时或淡化，目标至少 44×44 epx，Automation 暴露窗口状态、动作和错误。

## 现代化取舍

历史来源归 Archaeology；不得像素复制系统 Shell，必须保留系统菜单、Snap、Caption Buttons 和保留手势。

