# TreeDataGrid 规格

## 目标

提供宿主数据模型驱动的树形表格：负责树扁平化、筛选、异步子节点加载、可见行窗口、排序和选择；行模板、单元格编辑器与数据持久化由宿主负责。

## 非目标

不修改 `TreeDataGridNode.Value`，不创建窗口，不持久化排序/选择，不替宿主执行远程请求。

## 公共 API

`TreeDataGridNode` 提供稳定 `Key`、`Value`、`Parent`、`Children`、`HasChildren`、`IsExpanded`、`LoadState`、`LoadError` 和可取消 `ChildrenProvider`。控件提供 `SetRootItems`、`RefreshRows`、`SetViewport`、`SetExpandedAsync`、`LoadChildrenAsync`、`Select`、`ToggleSelection`、`ClearSelection`、`MoveSelection`、`SortBy`、`CommitCellEdit` 和 `InvokeRow`。

`SelectionMode=Single` 时只有一个选择；`Multiple` 支持 Ctrl+Space 切换；`Extended` 支持 Shift+上下扩展连续区间。`SelectedItem` 是当前锚点，`SelectedItems` 是只读快照。子节点加载状态为 `NotLoaded/Loading/Loaded/Failed`，取消或失败不会展开节点。

## 状态、筛选和虚拟化

`FilterText` 匹配节点 `Value.ToString()`；父节点在任一后代匹配时保留。`VisibleRows` 是逻辑扁平列表；`SetViewport(start,rowCount)` 只把窗口加 `RealizationBuffer` 范围暴露为 `RealizedRows`，不会为其他行创建容器。排序按列访问器稳定地作用于每层兄弟节点。

## 输入与失败

上下移动选择，Shift 扩展，左右折叠/展开，Enter 调用行，Ctrl+Space 切换多选。异步 provider 抛错时节点保留并进入 `Failed`，写入 `LoadError` 并发布 `ChildrenLoadFailed`；新加载会取消旧加载。空根列表显示 Empty，未知节点操作返回 `false`。

## 模板与无障碍

模板可提供 header、row repeater、滚动条和编辑器；缺少可选部件仍可使用模型 API。AutomationPeer 暴露 DataGrid 语义，行模板必须提供层级、展开状态、选中状态和可见焦点。

## 当前边界

工作项仍为 `in-progress/lab/P2`；列宽拖拽、冻结列和行内编辑器由后续规格单独锁定。
