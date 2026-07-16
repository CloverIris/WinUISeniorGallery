# File Card 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

把文件身份、预览、元数据和权限状态聚合在紧凑表面；加载渐进、动作权限感知、未知类型有一致回退。

## 现代化

重制为 Provider 无关 FileCard：宿主提供元数据、预览流与命令；控件不自行读取磁盘、OneDrive 或 SharePoint，也不执行危险操作。 使用当前 Fluent ThemeResource 和系统排版，不像素复刻历史 UI。

## 响应式与输入

卡片支持鼠标、触摸、键盘和屏幕阅读器；打开与更多操作分开，删除/移动等危险命令必须二次确认并由宿主执行。 窄屏采用折叠/分层，宽屏限制正文宽度；200% 文本缩放不裁切关键内容。

## 动效与无障碍

动效只说明上下文、层级或加载变化，Reduced Motion 即时切换。High Contrast 不依赖阴影/透明度，Automation 暴露角色、状态、来源和下一动作。
