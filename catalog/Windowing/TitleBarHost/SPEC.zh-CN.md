# TitleBarHost 规范

## 目标

提供可组合的 XAML 标题栏布局，生成拖动/交互区域并与宿主 WindowChrome 同步。

## 宿主与窗口边界

只拥有标题栏视觉与区域描述，不直接创建/关闭 AppWindow；宿主把区域交给 WindowChrome，并负责系统标题与窗口生命周期。

## 候选表面与闭锁条件

候选概念：Icon, Title, Subtitle, LeftContent, CenterContent, RightContent, PreferredHeight, RegionsChanged。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Unloaded --> Measuring --> Ready\nReady --> RegionsDirty --> Measuring\nAny --> HighContrast/Inactive\nAny --> Unloaded
```

## 模板部件与视觉树候选

候选树：RootGrid、IconPresenter、TextStack、Left/Center/RightPresenter、DragSurface、CaptionInsetSpacer。交互内容自动从拖动区域排除。

## 行为与失败模式

区域只在布局稳定后发布；内容卸载/隐藏即时移除排除区；标题过长省略但 Automation 保留全文。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

