# ConnectedTransitionBehavior 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given 目标在另一 Window，When Start，Then 使用淡化、焦点到目标且无共享 Visual。Given 导航取消，Then 源可见性/命中/焦点原子恢复。

## 输入、焦点与跨窗口生命周期

运行期间不让代理 Visual 获取焦点/命中；焦点由导航目标决定并在完成后设置。Back 导航可用反向配置但不阻塞。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

准备/启动 UI 各小于 4 ms，过渡 60 Hz 且无布局循环；超时默认候选 500 ms，100 次取消后无快照/Visual 泄漏。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

