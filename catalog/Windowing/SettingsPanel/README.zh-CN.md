# SettingsPanel

提供窗口内可分层导航的设置侧板，支持主设置→子设置→返回及未保存变更策略。

## 状态与范围

- 状态：proposed / lab / P2
- 依赖：contracts.windowing
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

不创建设置窗口、不保存设置、不决定账号/权限；宿主提供页面工厂、导航栈、提交/取消策略。

## Agent 所有权

仅 catalog/Windowing/SettingsPanel；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。

