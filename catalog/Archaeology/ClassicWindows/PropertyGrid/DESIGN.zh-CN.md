# Property Grid 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

选择驱动的反射式编辑、属性元数据与编辑器分离、批量选择显示公共属性、验证错误就地呈现、撤销由宿主事务管理。

## 现代化

重制为可扩展 PropertyGrid：宿主提供属性描述符、编辑器工厂和变更事务；控件负责分类、筛选、虚拟化、验证呈现与键盘导航。 外观采用当前 Fluent ThemeResource、系统排版和现代焦点视觉，不复制经典对话框像素。

## 响应式与输入

Tab 在名称/值/编辑器间移动，方向键展开层级，Enter 开始/提交，Esc 回滚；每行公开名称、类型、值、只读和验证状态。 窄屏使用折叠、溢出或单列，宽屏限制内容宽度；200% 文本缩放不得裁切提交和取消路径。

## 动效与无障碍

状态切换最长 200 ms，Reduced Motion 即时切换。High Contrast 使用系统颜色和非颜色提示；Automation 暴露结构、状态、验证和下一动作。
