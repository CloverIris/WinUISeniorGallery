# CompactOverlayHost

在同一个 AppWindow 上可靠切换 Inline 与 CompactOverlay Presenter，并保存/恢复尺寸和宿主确认状态。

## 状态与范围

- 状态：in-progress / lab / P1
- 依赖：contracts.windowing
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

不创建第二窗口、不迁移 XAML 内容、不控制播放；宿主提供既有 AppWindow、处理关闭并决定请求是否允许。

## Agent 所有权

仅 catalog/Windowing/CompactOverlayHost；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。
