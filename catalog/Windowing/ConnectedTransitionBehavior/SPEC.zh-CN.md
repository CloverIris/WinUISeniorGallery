# ConnectedTransitionBehavior 规范

## 目标

协调同一 Window 内源/目标元素的连接过渡，保留焦点和导航事务，并为跨窗口/缺失元素提供确定性淡化。

## 宿主与窗口边界

P0 候选仅同一 XamlRoot/Window；不移动真实 XAML、不跨窗口共享 CompositionVisual。宿主提供稳定 TransitionKey 和导航完成信号。

## 候选表面与闭锁条件

候选概念：TransitionKey, Role(Source/Target), Configuration, Prepare/Start/Cancel, TransitionCompleted。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Idle --> PreparedSource --> WaitingTarget --> Running --> Completed --> Idle\nAny --> Canceled/Fallback --> Idle
```

## 模板部件与视觉树候选

无固定模板；候选层为源快照/代理 Visual、目标占位和过渡遮罩。目标 ready 后才隐藏真实目标，结束后必恢复可见性。

## 行为与失败模式

相同 Key 一次只运行一个；导航取消、目标超时、Window 改变或 Reduced Motion 时走即时/淡化回退。任何失败 finally 恢复源/目标。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

