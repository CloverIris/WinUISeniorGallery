# RevealBorderBehavior

## 实现备注

当前实现公开指针几何、有效半径与透明度、Reduced Motion 和高对比度降级，以及可供模板使用的光晕矩形。行为只作用于附加元素，不创建 CompositionVisual，也不硬编码主题颜色。

为应用内可交互元素提供早期 Fluent 风格的指针邻近边框光效，并在现代主题/无障碍下安全降级。

## 状态与范围

- 状态：in-progress / lab / P2
- 依赖：contracts.motion
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

只附加到当前 Window 的 XAML 元素，不创建灯光窗口、不全局追踪指针、不改变命中测试或命令。

## Agent 所有权

仅 catalog/Windowing/RevealBorderBehavior；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。
