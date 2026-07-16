# Editable Breadcrumb 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

当前位置持续可见、任意祖先可直达、空间不足时折叠早期层级、显示模式与精确文本输入共存。

## 现代化

重制为 BreadcrumbBarEx：在基础 BreadcrumbBar 上增加编辑模式、历史/自动完成、层级菜单和可选拖放；路径解析与导航由宿主 Provider 负责。 外观采用当前 Fluent ThemeResource、系统排版和现代焦点视觉，不复制经典对话框像素。

## 响应式与输入

F4/Ctrl+L 进入编辑，Enter 提交，Esc 取消；鼠标、触摸、键盘与屏幕阅读器均可展开被折叠层级。 窄屏使用折叠、溢出或单列，宽屏限制内容宽度；200% 文本缩放不得裁切提交和取消路径。

## 动效与无障碍

状态切换最长 200 ms，Reduced Motion 即时切换。High Contrast 使用系统颜色和非颜色提示；Automation 暴露结构、状态、验证和下一动作。
