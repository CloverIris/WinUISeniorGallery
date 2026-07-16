# RevealBorderBehavior 设计

## 视觉层级与响应式

无模板部件；候选 Composition 树为 per-window shared light + per-element border mask。High Contrast/Reduced Motion 可禁用光效但保留标准边框。 窄/常规/宽和 100%–300% DPI 共用状态模型；窗口改变不重建宿主数据，200% 文本缩放优先重排/溢出。

## 输入与焦点

鼠标和笔 hover 有光效；触摸只用按下反馈；键盘/手柄焦点始终有独立标准 FocusVisual，不能依赖 Reveal。

## 主题、动效与无障碍

仅用 ThemeResource；High Contrast 不依赖材质/透明度。Reduced Motion 采用即时或淡化，目标至少 44×44 epx，Automation 暴露窗口状态、动作和错误。

## 现代化取舍

历史来源归 Archaeology；不得像素复制系统 Shell，必须保留系统菜单、Snap、Caption Buttons 和保留手势。

