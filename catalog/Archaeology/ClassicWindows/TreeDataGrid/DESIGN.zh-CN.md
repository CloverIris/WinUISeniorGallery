# Tree Data Grid 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

首列层级展开、其余列对齐比较、整行选择、列排序/调整、深层虚拟化和键盘树导航。

## 现代化

重制为数据源驱动 TreeDataGrid：宿主提供稳定节点 ID、异步子项和列定义；控件负责扁平化视口、虚拟化、选择、排序请求和展开状态。 外观采用当前 Fluent ThemeResource、系统排版和现代焦点视觉，不复制经典对话框像素。

## 响应式与输入

左右键折叠/展开或进入层级，上下键逐可见行，Home/End/Page 移动；屏幕阅读器获得行列索引、层级、展开和选择状态。 窄屏使用折叠、溢出或单列，宽屏限制内容宽度；200% 文本缩放不得裁切提交和取消路径。

## 动效与无障碍

状态切换最长 200 ms，Reduced Motion 即时切换。High Contrast 使用系统颜色和非颜色提示；Automation 暴露结构、状态、验证和下一动作。
