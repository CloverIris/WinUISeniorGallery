# MediaCenterGrid 设计

## 视觉层级

默认 16:9/2:3 海报、10-foot 可读文本和清晰焦点环；放大不遮挡邻项核心信息，窄窗口退化为内容条。

## 响应式

窄/常规/宽布局共用状态模型，Window 改变不重建数据；200% 文本缩放优先重排/溢出。

## 输入与焦点

手柄/D-pad 优先，A/Enter 调用、B/Esc 返回；鼠标/滚轮/触摸/键盘等价。焦点不进入未实现项，Automation 读行列、标题和操作。

## 主题、动效与无障碍

只用 ThemeResource；High Contrast 不仅靠透明度。Reduced Motion 即时切换，目标至少 44×44 epx，Automation 暴露状态/动作/错误。

## 现代化取舍

历史归 Archaeology；采用现代 Fluent/RTL/多输入，不复制历史资产或系统外观。

