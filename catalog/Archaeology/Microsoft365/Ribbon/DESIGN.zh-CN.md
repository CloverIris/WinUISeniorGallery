# Command Ribbon 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

按任务而非技术类别组织命令、可发现的大图标层级、上下文工具只在选中对象时出现、窗口变窄时按组逐级折叠。

## 现代化

现代化为可折叠 CommandRibbon：应用提供命令模型与上下文集合，控件负责分组、键盘提示、溢出和响应式布局；不复制 Office 图标、命令名或 Ribbon 视觉资产。 使用当前 Fluent ThemeResource 和系统排版，不像素复刻历史 UI。

## 响应式与输入

支持鼠标、触摸、键盘 KeyTip、方向键和屏幕阅读器分组导航；常用命令在 200% 文本缩放下仍可达。 窄屏采用折叠/分层，宽屏限制正文宽度；200% 文本缩放不裁切关键内容。

## 动效与无障碍

动效只说明上下文、层级或加载变化，Reduced Motion 即时切换。High Contrast 不依赖阴影/透明度，Automation 暴露角色、状态、来源和下一动作。
