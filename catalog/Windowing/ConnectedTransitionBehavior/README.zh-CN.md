# ConnectedTransitionBehavior

## 实现备注

当前实现提供序列号、运行/完成/取消状态、进度报告、中断、Reduced Motion 立即完成和附加控制器；它只协调 VisualStateManager，不跨窗口迁移内容。

协调同一 Window 内源/目标元素的连接过渡，保留焦点和导航事务，并为跨窗口/缺失元素提供确定性淡化。

## 状态与范围

- 状态：in-progress / lab / P2
- 依赖：contracts.motion
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

P0 候选仅同一 XamlRoot/Window；不移动真实 XAML、不跨窗口共享 CompositionVisual。宿主提供稳定 TransitionKey 和导航完成信号。

## Agent 所有权

仅 catalog/Windowing/ConnectedTransitionBehavior；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。
