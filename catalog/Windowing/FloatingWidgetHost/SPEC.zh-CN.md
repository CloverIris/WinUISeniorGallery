# FloatingWidgetHost 规范

## 目标

创建并管理应用拥有的次级浮动小组件窗口，支持置顶、最小化和主窗口生命周期策略。

## 宿主与窗口边界

Host 拥有次级 AppWindow 和窗口级服务，但内容由宿主 ContentFactory 在目标 DispatcherQueue 创建；禁止把主窗口 XAML 实例重父级化。

## 候选表面与闭锁条件

候选概念：OwnerWindow, ContentFactory, WidgetId, Placement, IsAlwaysOnTop, OwnerClosePolicy, Open/Close/Restore。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Closed --> Creating --> Visible\nVisible --> Minimized/Hidden --> Visible\nCreating/Visible --> Failed\nAny --> Closing --> Closed
```

## 模板部件与视觉树候选

每个窗口候选树：WindowChrome、WidgetTitleBar、ContentPresenter、Loading/Error Presenter；主窗口只保留轻量句柄和状态。

## 行为与失败模式

同 WidgetId Open 幂等；创建失败不发布句柄。Owner 关闭按 Close/KeepAlive/Transfer 策略执行，Transfer 失败安全关闭。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

