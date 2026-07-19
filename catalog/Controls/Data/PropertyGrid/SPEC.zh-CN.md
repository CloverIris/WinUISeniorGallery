# PropertyGrid Specification

## Goal

目标：提供反射元数据、宿主编辑器、校验和可逆事务的属性面板。对象生命周期始终由宿主拥有。

## Non-goals

不负责创建对象、持久化配置、执行网络操作或替宿主实现自定义编辑器。

## Public API

`SelectedObject`、`Properties`、`Groups`、`FilterText`、`SortMode`、`IsReadOnly`、`EditorProvider`、`BeginEdit`、`CommitEdit`、`CancelEdit`、`TrySetValue`、`Undo`、`Redo`、`ClearHistory`、`CanUndo`、`CanRedo`。成功编辑进入 Undo 栈；新编辑清空 Redo 栈。

## State model

`Empty/Ready/Editing/Error`。校验失败保留旧模型值；Undo/Redo 重新经过同一转换和验证管线，失败时不移动历史栈。

## Template parts and visual tree

Not locked.

## Behavior and failure modes

模板缺失时纯逻辑 API 仍可用；可视化编辑器通过宿主模板绑定 `PropertyGridProperty.Value` 和 `ValidationError`。自动化应报告当前对象、属性数和 Undo/Redo 可用性。

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Proposed implementation baseline
`SelectedObject`, `Properties`, `EditorProvider`, `SortMode=Categorized`, `FilterText`, `IsReadOnly`; states `Empty/Ready/Editing/Error`, parts toolbar/tree/description/editor. Enter提交、Esc回滚；验证失败不写模型。
