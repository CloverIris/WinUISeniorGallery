# Wizard / Stepper 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

渐进收集信息、每步验证、提交前可回退、不可逆动作前明确确认、完成/失败提供后续路径。

## 现代化

重制为 WizardStepper：宿主声明步骤图、验证与提交命令；控件提供横向/纵向进度、保存恢复、分支跳转和焦点管理。 外观采用当前 Fluent ThemeResource、系统排版和现代焦点视觉，不复制经典对话框像素。

## 响应式与输入

键盘 Tab 顺序保持在当前步骤，Alt+Left/Back 返回但不绕过验证；屏幕阅读器公告步骤序号、标题、错误和提交状态。 窄屏使用折叠、溢出或单列，宽屏限制内容宽度；200% 文本缩放不得裁切提交和取消路径。

## 动效与无障碍

状态切换最长 200 ms，Reduced Motion 即时切换。High Contrast 使用系统颜色和非颜色提示；Automation 暴露结构、状态、验证和下一动作。
