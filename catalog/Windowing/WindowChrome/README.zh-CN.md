# WindowChrome

协调单个 Window 的自定义标题栏、系统按钮区域、背景材质和非客户区命中测试。

## 状态与范围

- 状态：in-progress / lab / P1
- 依赖：contracts.windowing
- WindowChrome 仍处于候选实现阶段；`WindowPlacementCoordinator` 已作为纯主机中立几何状态机落地，供 Snap/自定义标题栏实验使用。

`WindowPlacementCoordinator` 只维护工作区、边界、最小/最大尺寸、Normal/Maximized/FullScreen、移动、八方向调整大小和八种 Snap 目标；它不会调用 AppWindow/HWND，宿主必须自行应用 `Snapshot`。

## 宿主与窗口边界

一实例绑定一个宿主 Window/AppWindow；不创建窗口、不拥有页面内容、不改变业务导航。宿主负责实例保存、Closed 调用和错误展示。

## Agent 所有权

仅 catalog/Windowing/WindowChrome；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。
