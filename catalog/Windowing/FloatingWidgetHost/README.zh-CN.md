# FloatingWidgetHost

创建并管理应用拥有的次级浮动小组件窗口，支持置顶、最小化和主窗口生命周期策略。

## 状态与范围

- 状态：in-progress / lab / P1
- 依赖：contracts.windowing
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

Host 拥有次级 AppWindow 和窗口级服务，但内容由宿主 ContentFactory 在目标 DispatcherQueue 创建；禁止把主窗口 XAML 实例重父级化。

## Agent 所有权

仅 catalog/Windowing/FloatingWidgetHost；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。
