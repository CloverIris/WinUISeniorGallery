# CompactOverlayHost 设计

## 视觉层级与响应式

候选为非视觉 Host；可选 OverlayChromePresenter 仅显示退出、置顶说明和最小控制。不得复制页面内容。 窄/常规/宽和 100%–300% DPI 共用状态模型；窗口改变不重建宿主数据，200% 文本缩放优先重排/溢出。

## 输入与焦点

Overlay 必有键盘/触摸/鼠标退出路径；Esc 行为由宿主策略决定且不能关闭 Window；焦点保持在可见内容。

## 主题、动效与无障碍

仅用 ThemeResource；High Contrast 不依赖材质/透明度。Reduced Motion 采用即时或淡化，目标至少 44×44 epx，Automation 暴露窗口状态、动作和错误。

## 现代化取舍

历史来源归 Archaeology；不得像素复制系统 Shell，必须保留系统菜单、Snap、Caption Buttons 和保留手势。

