# CompactOverlayHost 规范

## 目标

在同一个 AppWindow 上可靠切换 Inline 与 CompactOverlay Presenter，并保存/恢复尺寸和宿主确认状态。

## 宿主与窗口边界

不创建第二窗口、不迁移 XAML 内容、不控制播放；宿主提供既有 AppWindow、处理关闭并决定请求是否允许。

## 候选表面与闭锁条件

候选概念：AppWindow, RequestedMode, ConfirmedMode, PreferredOverlaySize, Enter/ExitRequested, TransitionFailed。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Inline --> EnteringOverlay --> Overlay\nOverlay --> ExitingOverlay --> Inline\nTransition --> Failed --> previous confirmed mode\nAny --> Closing --> Closed
```

## 模板部件与视觉树候选

候选为非视觉 Host；可选 OverlayChromePresenter 仅显示退出、置顶说明和最小控制。不得复制页面内容。

## 行为与失败模式

同类请求合并，反向请求串行等待；只有 SetPresenter 成功和状态确认后更新 ConfirmedMode，失败恢复原尺寸/Presenter。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

