# WindowChrome 规范

## 目标

协调单个 Window 的自定义标题栏、系统按钮区域、背景材质和非客户区命中测试。

## 宿主与窗口边界

一实例绑定一个宿主 Window/AppWindow；不创建窗口、不拥有页面内容、不改变业务导航。宿主负责实例保存、Closed 调用和错误展示。

## 候选表面与闭锁条件

候选概念：Window, TitleBarContent, BackdropKind, ExtendIntoTitleBar, InteractiveRegions, Attach/Detach, ChromeState。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Detached --> Attaching --> Active\nActive --> Reconfiguring --> Active\nAny --> Failed --> Detached\nActive/Failed --> Closing --> Closed
```

## 模板部件与视觉树候选

候选树：ChromeRoot、TitleBarHost、ContentPresenter、BackdropLayer；系统 CaptionButtons 默认仍由系统绘制，部件名 ready 前冻结。

## 行为与失败模式

Attach 幂等且跨 Window 重绑先 Detach；DPI/尺寸/主题改变同一批次重算拖动与排除区；Closed 后所有调用失败为可观察状态。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

