# MiniPlayerHost 设计

## 视觉层级

迷你态优先封面/缩略、标题、播放暂停和展开；目标不低于 280×72 epx，更窄退化为图标条。

## 响应式

窄/常规/宽布局共用状态模型，Window 改变不重建数据；200% 文本缩放优先重排/溢出。

## 输入与焦点

折叠后焦点到语义最接近的迷你元素，展开恢复原焦点；Esc 不隐式折叠，拖动仅在 DragRegion 生效。

## 主题、动效与无障碍

只用 ThemeResource；High Contrast 不仅靠透明度。Reduced Motion 即时切换，目标至少 44×44 epx，Automation 暴露状态/动作/错误。

## 现代化取舍

历史归 Archaeology；采用现代 Fluent/RTL/多输入，不复制历史资产或系统外观。

