# Property Grid 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Visual Basic 与 Visual Studio 的属性窗口长期使用分类/字母排序双模式、名称-值两列、嵌套对象、描述区和类型专用编辑器编辑当前选择。

## 设计基因

选择驱动的反射式编辑、属性元数据与编辑器分离、批量选择显示公共属性、验证错误就地呈现、撤销由宿主事务管理。

## 现代化重制

重制为可扩展 PropertyGrid：宿主提供属性描述符、编辑器工厂和变更事务；控件负责分类、筛选、虚拟化、验证呈现与键盘导航。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.property-grid（PropertyGrid）拥有。 展项不声明公共 API、类型或资源键，不复制稳定控件；历史标签仅用于说明来源。

## 状态与失败模式

NoSelection、Single、Multiple、Editing、Validating、ReadOnly、Error；编辑对象被移除时取消事务并清除编辑器。 数据规模、格式或 Provider 异常必须局部降级，不损坏已提交值、选择或宿主数据。
