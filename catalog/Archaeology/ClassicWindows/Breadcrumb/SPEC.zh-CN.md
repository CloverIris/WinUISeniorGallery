# Editable Breadcrumb 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Windows Vista 文件资源管理器把地址栏路径拆成可点击层级段，同时保留切换到文本路径、层级下拉和历史导航的能力。

## 设计基因

当前位置持续可见、任意祖先可直达、空间不足时折叠早期层级、显示模式与精确文本输入共存。

## 现代化重制

重制为 BreadcrumbBarEx：在基础 BreadcrumbBar 上增加编辑模式、历史/自动完成、层级菜单和可选拖放；路径解析与导航由宿主 Provider 负责。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.breadcrumb-bar-ex（BreadcrumbBarEx）拥有。 展项不声明公共 API、类型或资源键，不复制稳定控件；历史标签仅用于说明来源。

## 状态与失败模式

Display、Editing、SuggestionsOpen、Navigating、Invalid；Esc 取消编辑并还原原路径，导航失败保留输入且显示可重试错误。 数据规模、格式或 Provider 异常必须局部降级，不损坏已提交值、选择或宿主数据。
