# RevealBorderBehavior 规范

## 目标

为应用内可交互元素提供早期 Fluent 风格的指针邻近边框光效，并在现代主题/无障碍下安全降级。

## 宿主与窗口边界

只附加到当前 Window 的 XAML 元素，不创建灯光窗口、不全局追踪指针、不改变命中测试或命令。

## 候选表面与闭锁条件

候选概念：IsEnabled, Radius, Intensity, BorderBrush, PressedIntensity, UseSharedLight。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Detached --> Idle --> PointerNear --> PointerOver --> Pressed\nAny --> Disabled/HighContrast/Unloaded
```

## 模板部件与视觉树候选

无模板部件；候选 Composition 树为 per-window shared light + per-element border mask。High Contrast/Reduced Motion 可禁用光效但保留标准边框。

## 行为与失败模式

只对可见/启用/命中元素计算；Window 失焦或 PointerExited 清除。共享光按弱引用注册，卸载立即注销。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

