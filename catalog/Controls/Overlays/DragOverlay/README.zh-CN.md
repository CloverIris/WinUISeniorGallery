# DragOverlay

`DragOverlay` 是宿主驱动的拖放反馈层。它只呈现预览、说明、位置和允许性状态，不接管拖放数据、命中测试、窗口创建或跨窗口坐标转换。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Overlays/DragOverlay

实现目录：`src/WinUI3.Senior.Controls/Overlays/DragOverlay`

## 实现准备
当前实现覆盖 `Show`、`Update`、`BeginDrop`、`Hide` 与 `Hidden/Allowed/Forbidden/Dropping` 状态。`DropRequested` 必须由宿主明确接受才会结束预览，空操作集显示禁止状态；控件默认不参与命中测试并在隐藏时清除源对象与预览引用。跨窗口坐标和 Escape 取消仍由宿主负责，Gallery 接入与自动化验证后续补齐。
