# RevealBorderBehavior 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given 键盘焦点且无鼠标，Then FocusVisual 清晰无 Reveal 依赖。Given High Contrast，Then 动态光关闭而静态边框达到系统对比。

## 输入、焦点与跨窗口生命周期

鼠标和笔 hover 有光效；触摸只用按下反馈；键盘/手柄焦点始终有独立标准 FocusVisual，不能依赖 Reveal。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

每 Window 一个共享 light，500 可见元素时每帧 UI 小于 2 ms、无布局；不可见元素零更新，卸载后 Composition 引用归零。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

