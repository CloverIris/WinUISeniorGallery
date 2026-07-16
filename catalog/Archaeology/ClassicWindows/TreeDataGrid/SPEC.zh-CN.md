# Tree Data Grid 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Windows 管理工具、资源管理器和开发工具长期把树形层级与报告视图列组合，用于进程、项目、对象和资源的层级数据；这是跨产品模式，不归因于单一原创控件。

## 设计基因

首列层级展开、其余列对齐比较、整行选择、列排序/调整、深层虚拟化和键盘树导航。

## 现代化重制

重制为数据源驱动 TreeDataGrid：宿主提供稳定节点 ID、异步子项和列定义；控件负责扁平化视口、虚拟化、选择、排序请求和展开状态。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.tree-data-grid（TreeDataGrid）拥有。 展项不声明公共 API、类型或资源键，不复制稳定控件；历史标签仅用于说明来源。

## 状态与失败模式

LoadingRoot、Ready、ExpandingNode、Partial、Empty、Error；单节点加载失败只影响该分支，重试不折叠其他分支。 数据规模、格式或 Provider 异常必须局部降级，不损坏已提交值、选择或宿主数据。
