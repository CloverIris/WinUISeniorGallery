# EdgeCommandPanel 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given 非模态半开，When Window 缩窄，Then extent 钳制且内容仍可聚焦。Given 捕获丢失，Then 面板落到唯一稳定态并公告。

## 输入、焦点与跨窗口生命周期

显式按钮、键盘和触摸把手均可打开；Esc/B 关闭最内层，边缘手势仅在应用声明热区且不与系统保留手势冲突。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

拖动 60 Hz、UI 每帧小于 3 ms；100 个命令用 ItemsRepeater 虚拟化，关闭后无指针捕获。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

