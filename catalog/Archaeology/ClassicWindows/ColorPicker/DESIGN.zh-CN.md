# Advanced Color Picker 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

视觉探索与数值精确输入并存、颜色空间同步、最近/自定义色记忆、透明度可选、无效文本即时验证。

## 现代化

重制为 ColorPickerEx：在现代 ColorPicker 上增加色板、历史、收藏、取色 Provider 与可配置色域；屏幕取色权限和色彩管理由宿主负责。 外观采用当前 Fluent ThemeResource、系统排版和现代焦点视觉，不复制经典对话框像素。

## 响应式与输入

颜色通道可键盘精调，色谱支持二维 Automation 值与文本描述；不能只用色块表达当前值，必须显示可复制的数值。 窄屏使用折叠、溢出或单列，宽屏限制内容宽度；200% 文本缩放不得裁切提交和取消路径。

## 动效与无障碍

状态切换最长 200 ms，Reduced Motion 即时切换。High Contrast 使用系统颜色和非颜色提示；Automation 暴露结构、状态、验证和下一动作。
