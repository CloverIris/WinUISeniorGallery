# FloatingWidgetHost 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given owner 关闭策略 Close，When 主窗口关闭，Then 所有浮窗先取消任务再关闭。Given ContentFactory 抛错，Then 无幽灵窗口/句柄且可重试。

## 输入、焦点与跨窗口生命周期

浮窗有独立焦点域、Alt+F4/系统菜单/键盘关闭；置顶不阻止切换应用，关闭后焦点仅在 owner 仍存在时恢复。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

空浮窗创建目标小于 500 ms，拖动/缩放 60 Hz；关闭 100 个窗口后 AppWindow、DispatcherQueue 回调和内容引用归零。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

