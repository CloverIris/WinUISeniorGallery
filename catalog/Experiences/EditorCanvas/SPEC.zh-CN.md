# EditorCanvas Specification

## Goal

目标：提供一个宿主中立的编辑画布入口。Core 负责不可变文档快照、线程安全修改、视口数学和压感笔画聚合；WinUI 控件负责模板、命中测试、输入捕获和可视对象渲染。文件解析、序列化和协作不属于本阶段。

## Non-goals

本阶段不实现文件导入、页面缩略图、标尺/参考线持久化、C++ 原生渲染、网络协作或云同步。

## Public API

Core：`CanvasPoint`、`CanvasSize`、`CanvasRect`、`CanvasTransform`、`CanvasObject`、`CanvasDocumentSnapshot`、`CanvasDocumentController`、`CanvasViewportController`、`CanvasPressureSample`、`CanvasStroke`、`CanvasInkSession`。WinUI：`EditorCanvas`、`EditorCanvasTool`。`Document` 和 `InkSession` 由宿主显式设置；控件不创建全局文档或原生笔设备。

## State model

`Document` 初始为空或由宿主提供。控件工具为 `Select`、`Pan`、`Rectangle`、`Text`、`Ink`；鼠标中键/ Pan 工具平移，滚轮以指针位置缩放，箭头移动选区，Delete 删除未锁定选区，Esc 清空选区。Rectangle/Text 在拖拽释放时插入本地对象；Ink 将 `CanvasPressureSample` 聚合为 `CanvasStroke`，释放时一次性插入文档，取消时不产生对象。选区和视口变化必须可观察，修改以 Core `Revision` 单调递增。

## Template parts and visual tree

必需 `PART_Surface`；可选 `PART_Grid`、`PART_SelectionAdorner`。缺失 Surface 时控件保留文档操作但不渲染；可选部件缺失只关闭对应视觉层。对象渲染使用 Canvas 子元素并设置 Automation Name，不依赖 payload 的具体类型。

## Behavior and failure modes

文档为空时显示空画布；非法几何或重复 ID 被 Core 归一化/拒绝；宿主卸载时应解除 `Document`，控件不持有窗口或文件句柄。只读状态拒绝插入、移动和删除，但仍允许缩放、平移和观察选区。

## Open Decisions

Open Decisions：页面/图层持久化格式、100k 对象的 retained-mode 渲染预算、C++ Canvas.Native 接入点、压感笔 ABI。

## 场景、数据与视觉树
模型 Document→Page/Layer→Element；树 `CanvasWinUI→Rulers/Guides→ThumbnailRail`；状态 Loading/Ready/Panning/Editing/Saving/Error。当前仅场景契约，不定义Canvas API。
