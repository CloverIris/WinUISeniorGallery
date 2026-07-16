# DockLayoutPreview 规范

## 目标

在应用内部拖拽内容时显示可投放 Dock 区域与磁吸预览，不复制 Windows 系统 Snap Layouts。

## 宿主与窗口边界

只布局应用内部面板，不移动 OS 窗口、不调用系统 Snap、不覆盖标题栏悬停菜单。宿主拥有拖拽对象和最终布局事务。

## 候选表面与闭锁条件

候选概念：LayoutModel, DraggedItem, Targets, ActiveTarget, PreviewBounds, Commit/CancelRequested, SnapThreshold。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Idle --> DragTracking --> Previewing --> Committing --> Idle\nDragTracking/Previewing --> Canceled --> Idle\nAny --> Failed
```

## 模板部件与视觉树候选

候选树：OverlayRoot、TargetRepeater、PreviewRectangle、InvalidDropPresenter；覆盖层 IsHitTestVisible 仅在拖拽期。

## 行为与失败模式

目标由宿主布局模型生成；commit 是单一事务，失败回滚原位置。跨 DPI/显示器仅用于应用窗口内坐标转换。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

