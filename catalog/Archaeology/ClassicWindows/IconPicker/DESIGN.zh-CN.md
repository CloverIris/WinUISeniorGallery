# Icon Picker 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

大集合网格浏览、当前选择明确、来源路径可切换、图标预览与索引分离、键盘快速定位。

## 现代化

重制为安全 IconPicker：支持应用注册的 Symbol/IconSource 目录、搜索、分类、收藏和 RTL 元数据；默认不扫描任意 DLL/EXE 或提取系统资源。 外观采用当前 Fluent ThemeResource、系统排版和现代焦点视觉，不复制经典对话框像素。

## 响应式与输入

虚拟化网格支持方向键、Home/End、Page 和文字搜索；每个图标公开名称、类别、镜像能力和来源，不能只读 Unicode 值。 窄屏使用折叠、溢出或单列，宽屏限制内容宽度；200% 文本缩放不得裁切提交和取消路径。

## 动效与无障碍

状态切换最长 200 ms，Reduced Motion 即时切换。High Contrast 使用系统颜色和非颜色提示；Automation 暴露结构、状态、验证和下一动作。
