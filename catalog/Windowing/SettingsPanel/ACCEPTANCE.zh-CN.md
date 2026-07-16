# SettingsPanel 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given 子页有未保存值，When Esc，Then 先触发确认且不弹出两次。Given Window 关闭，Then 取消导航/保存并不阻塞 Closed。

## 输入、焦点与跨窗口生命周期

Alt+Left/Backspace/B 返回，Esc 关闭当前弹层后面板；模态焦点约束，关闭恢复触发元素，错误聚焦首个无效字段。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

打开首帧小于 100 ms，页面切换小于 150 ms；关闭后页面订阅释放，缓存上限由宿主策略锁定。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

