# DockLayoutPreview

在应用内部拖拽内容时显示可投放 Dock 区域与磁吸预览，不复制 Windows 系统 Snap Layouts。

## 状态与范围

- 状态：proposed / lab / P2
- 依赖：contracts.windowing
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

只布局应用内部面板，不移动 OS 窗口、不调用系统 Snap、不覆盖标题栏悬停菜单。宿主拥有拖拽对象和最终布局事务。

## Agent 所有权

仅 catalog/Windowing/DockLayoutPreview；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。

