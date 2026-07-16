# Search Suggestions 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

输入即反馈、来源分组、键盘预览选择、查询与建议提交区分、无结果也提供明确反馈。

## 现代化

重制为 SearchBoxEx：宿主注入可取消的多来源 Provider，控件合并分组、最近项和趋势项；不默认上传查询或访问 Windows 搜索索引。 外观采用当前 Fluent ThemeResource、系统排版和现代焦点视觉，不复制经典对话框像素。

## 响应式与输入

上下键浏览、Enter 提交、Esc 关闭建议/清空分层处理；触摸和屏幕阅读器显示类别、来源与结果类型。 窄屏使用折叠、溢出或单列，宽屏限制内容宽度；200% 文本缩放不得裁切提交和取消路径。

## 动效与无障碍

状态切换最长 200 ms，Reduced Motion 即时切换。High Contrast 使用系统颜色和非颜色提示；Automation 暴露结构、状态、验证和下一动作。
