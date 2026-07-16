# Advanced Color Picker

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 定位

Win32 ChooseColor 常用颜色对话框与后来 UWP/WinUI ColorPicker 提供基础色板、自定义色、RGB/HSV/HEX 精确输入和颜色光谱选择。

## 状态

- 工作项：specified
- 成熟度：lab
- 优先级：P1
- 许可审查：pending

## 边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.color-picker-ex（ColorPickerEx）拥有。 不提交 Windows 历史截图、图标、字体、声音或提取资源。

## 文档

- SPEC：原始体验与现代边界
- DESIGN：保留、改造和无障碍
- INTEGRATION：稳定依赖与平台边界
- ACCEPTANCE：研究和行为验收
- SOURCES：出处与资产状态
