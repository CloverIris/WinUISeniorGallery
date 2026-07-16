# SettingsPanel 规范

## 目标

提供窗口内可分层导航的设置侧板，支持主设置→子设置→返回及未保存变更策略。

## 宿主与窗口边界

不创建设置窗口、不保存设置、不决定账号/权限；宿主提供页面工厂、导航栈、提交/取消策略。

## 候选表面与闭锁条件

候选概念：IsOpen, RootPage, NavigationStack, PaneWidth, IsModal, Navigate/Back/CloseRequested, UnsavedChangesPolicy。这些不是公共 API；进入 ready 前冻结类型、默认值、事件取消/线程语义和失败结果。

## 状态图

```text
Closed --> Opening --> Root\nRoot <--> Child\nRoot/Child --> ConfirmingChanges --> Closing --> Closed\nAny --> Failed
```

## 模板部件与视觉树候选

候选树：Scrim、PaneRoot、Header、BackButton、FramePresenter、Footer、FocusSentinels、ErrorPresenter。

## 行为与失败模式

导航异步可取消；重复打开保留当前栈或按宿主策略复位。关闭遇未保存变更先确认，确认期间禁止二次关闭。

## Ready 晋级门禁

冻结 API、状态转换表、部件/非视觉 Contract、窗口 Closed/取消/回滚、资源键、平台版本降级、性能和 Automation，并同步中英文 API/ID 后才可 ready。

