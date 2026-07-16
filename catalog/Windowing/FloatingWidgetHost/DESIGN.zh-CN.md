# FloatingWidgetHost 设计

## 视觉层级与响应式

每个窗口候选树：WindowChrome、WidgetTitleBar、ContentPresenter、Loading/Error Presenter；主窗口只保留轻量句柄和状态。 窄/常规/宽和 100%–300% DPI 共用状态模型；窗口改变不重建宿主数据，200% 文本缩放优先重排/溢出。

## 输入与焦点

浮窗有独立焦点域、Alt+F4/系统菜单/键盘关闭；置顶不阻止切换应用，关闭后焦点仅在 owner 仍存在时恢复。

## 主题、动效与无障碍

仅用 ThemeResource；High Contrast 不依赖材质/透明度。Reduced Motion 采用即时或淡化，目标至少 44×44 epx，Automation 暴露窗口状态、动作和错误。

## 现代化取舍

历史来源归 Archaeology；不得像素复制系统 Shell，必须保留系统菜单、Snap、Caption Buttons 和保留手势。

