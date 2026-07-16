# TabTearOutBehavior 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given 目标 ContentFactory 失败，When TearOut，Then 源 tab 索引/选择/焦点不变且目标关闭。Given 成功，Then 任一时刻恰好一个宿主拥有模型。

## 输入、焦点与跨窗口生命周期

指针拖出、键盘“移到新窗口”和屏幕阅读器命令等价；拖动取消恢复源，目标关闭按宿主策略回附或关闭文档。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

拖动反馈 60 Hz；目标空窗创建目标 500 ms，事务提交 UI 小于 16 ms；100 次转移无窗口/模型订阅泄漏。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

