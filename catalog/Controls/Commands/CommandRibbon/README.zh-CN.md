# CommandRibbon

`CommandRibbon` 是由宿主注入命令模型的上下文命令带。控件负责标签、分组、KeyTip、响应式密度和溢出顺序，命令副作用仍由宿主的 `ICommand` 负责。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Commands/CommandRibbon

实现目录：`src/WinUI3.Senior.Controls/Commands/CommandRibbon`

## 实现准备
当前实现覆盖：`CommandRibbon`、`CommandRibbonTab`、`CommandRibbonGroup`、`CommandRibbonCommand`；支持 `AlwaysExpanded`、`AlwaysMinimized`、`Auto` 三种折叠策略，稳定的优先级溢出排序、KeyTip 唯一调用、禁用命令保护和可观察布局结果。控件不创建窗口、不持久化命令、不引用 Office 资源。仍处于持续实现阶段，Gallery 页面和自动化验证在后续批次补齐。
