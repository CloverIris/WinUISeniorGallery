# ConnectedTransitionBehavior 设计

## 视觉层级与响应式

无固定模板；候选层为源快照/代理 Visual、目标占位和过渡遮罩。目标 ready 后才隐藏真实目标，结束后必恢复可见性。 窄/常规/宽和 100%–300% DPI 共用状态模型；窗口改变不重建宿主数据，200% 文本缩放优先重排/溢出。

## 输入与焦点

运行期间不让代理 Visual 获取焦点/命中；焦点由导航目标决定并在完成后设置。Back 导航可用反向配置但不阻塞。

## 主题、动效与无障碍

仅用 ThemeResource；High Contrast 不依赖材质/透明度。Reduced Motion 采用即时或淡化，目标至少 44×44 epx，Automation 暴露窗口状态、动作和错误。

## 现代化取舍

历史来源归 Archaeology；不得像素复制系统 Shell，必须保留系统菜单、Snap、Caption Buttons 和保留手势。

