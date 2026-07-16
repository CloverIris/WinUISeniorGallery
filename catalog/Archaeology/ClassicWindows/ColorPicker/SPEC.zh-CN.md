# Advanced Color Picker 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Win32 ChooseColor 常用颜色对话框与后来 UWP/WinUI ColorPicker 提供基础色板、自定义色、RGB/HSV/HEX 精确输入和颜色光谱选择。

## 设计基因

视觉探索与数值精确输入并存、颜色空间同步、最近/自定义色记忆、透明度可选、无效文本即时验证。

## 现代化重制

重制为 ColorPickerEx：在现代 ColorPicker 上增加色板、历史、收藏、取色 Provider 与可配置色域；屏幕取色权限和色彩管理由宿主负责。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.color-picker-ex（ColorPickerEx）拥有。 展项不声明公共 API、类型或资源键，不复制稳定控件；历史标签仅用于说明来源。

## 状态与失败模式

Spectrum、Numeric、Palette、Eyedropper、Invalid、Unavailable；不同输入同步使用单一规范颜色，编辑中的非法文本不污染已提交值。 数据规模、格式或 Provider 异常必须局部降级，不损坏已提交值、选择或宿主数据。
