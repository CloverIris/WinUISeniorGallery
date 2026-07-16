# EdgeCommandPanel

在应用窗口边缘呈现可拖拽展开的命令面板，提炼 Charms/Guide 的渐进披露而不模拟系统边缘 UI。

## 状态与范围

- 状态：proposed / lab / P2
- 依赖：contracts.windowing
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

仅在宿主 XAML 根内工作，不注册全局边缘手势、不覆盖其他应用、不调用系统 Charms。宿主提供命令和打开策略。

## Agent 所有权

仅 catalog/Windowing/EdgeCommandPanel；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。

