# TabTearOutBehavior 规范

## 目标

把标签页的数据/视图模型从一个窗口事务化转移到新窗口或另一 TabHost，并支持失败回滚。

## 宿主与窗口边界

绝不跨 Window 重父级化 XAML 元素；源/目标宿主提供序列化描述、ContentFactory 和窗口工厂。Behavior 只协调事务和焦点。

## 候选表面与闭锁条件

候选概念：SourceHost, WindowFactory, TransferDescriptor, CanTearOut, TearOut/AttachRequested, TransferFailed。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Attached --> Dragging --> PreparingTransfer --> CreatingTarget --> Committing --> AttachedTarget\nAnyTransfer --> RollingBack --> AttachedSource\nAny --> Closed
```

## 模板部件与视觉树候选

候选视觉只含 DragAdorner、DropTargets、TransferPlaceholder；真实内容在目标 DispatcherQueue 由 ContentFactory 新建。

## 行为与失败模式

两阶段提交：目标窗口与内容 ready 后源才移除；任一步失败关闭目标并恢复源索引/选择/焦点。重复同 tab 请求串行化。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

