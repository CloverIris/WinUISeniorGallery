# EdgeCommandPanel 规范

## 目标

在应用窗口边缘呈现可拖拽展开的命令面板，提炼 Charms/Guide 的渐进披露而不模拟系统边缘 UI。

## 宿主与窗口边界

仅在宿主 XAML 根内工作，不注册全局边缘手势、不覆盖其他应用、不调用系统 Charms。宿主提供命令和打开策略。

## 候选表面与闭锁条件

候选概念：Edge, ItemsSource, IsOpen, PeekExtent, ExpandedExtent, IsModal, Open/CloseRequested。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Closed --> Peeking --> Dragging --> Open\nOpen --> Dragging --> Closed\nAny --> Failed/Unloaded
```

## 模板部件与视觉树候选

候选树：Scrim、PanelRoot、DragHandle、CommandItemsRepeater、Header/FooterPresenter、FocusSentinel。

## 行为与失败模式

拖动按阈值/速度决定停靠；模态态限制焦点，非模态不遮断内容。尺寸变化重算 extent 并钳制当前进度。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

