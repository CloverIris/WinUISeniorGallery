# TitleBarHost

提供可组合的 XAML 标题栏布局，生成拖动/交互区域并与宿主 WindowChrome 同步。

## 状态与范围

- 状态：proposed / lab / P1
- 依赖：contracts.windowing
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

只拥有标题栏视觉与区域描述，不直接创建/关闭 AppWindow；宿主把区域交给 WindowChrome，并负责系统标题与窗口生命周期。

## Agent 所有权

仅 catalog/Windowing/TitleBarHost；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。

