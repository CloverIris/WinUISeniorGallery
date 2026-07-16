# WindowChrome

协调单个 Window 的自定义标题栏、系统按钮区域、背景材质和非客户区命中测试。

## 状态与范围

- 状态：proposed / lab / P1
- 依赖：contracts.windowing
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

一实例绑定一个宿主 Window/AppWindow；不创建窗口、不拥有页面内容、不改变业务导航。宿主负责实例保存、Closed 调用和错误展示。

## Agent 所有权

仅 catalog/Windowing/WindowChrome；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。

