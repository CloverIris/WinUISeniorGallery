# Command Ribbon 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Office 2007 用 Fluent Ribbon 替代层层菜单和工具栏，以选项卡、分组、大小层级和上下文选项卡把高价值命令直接呈现。

## 设计基因

按任务而非技术类别组织命令、可发现的大图标层级、上下文工具只在选中对象时出现、窗口变窄时按组逐级折叠。

## 现代化重制

现代化为可折叠 CommandRibbon：应用提供命令模型与上下文集合，控件负责分组、键盘提示、溢出和响应式布局；不复制 Office 图标、命令名或 Ribbon 视觉资产。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.command-ribbon（CommandRibbon）拥有。 展项不声明公共 API，不复制现代组件；历史名称只用于研究标题，演示标注“现代化重制”。

## 状态与失败模式

Expanded、Simplified、Collapsed、Contextual、Overflow；上下文消失时焦点移动到同组选项卡或内容区，不能落入已卸载命令。 数据、服务或依赖缺失时必须显示局部且可解释的降级，不能用假数据冒充真实账号结果。
