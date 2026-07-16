# CompactOverlayHost 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given Overlay 不支持，When 请求进入，Then 只发失败、保持 Inline/原尺寸/焦点。Given Entering 时关闭 Window，Then 不尝试回退已关闭 AppWindow。

## 输入、焦点与跨窗口生命周期

Overlay 必有键盘/触摸/鼠标退出路径；Esc 行为由宿主策略决定且不能关闭 Window；焦点保持在可见内容。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

切换不重建根视觉树，确认延迟目标小于 250 ms；100 次切换无 Presenter/事件泄漏。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

