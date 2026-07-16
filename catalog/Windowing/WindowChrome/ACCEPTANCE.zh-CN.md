# WindowChrome 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given 标题栏有搜索框，When 窗口缩放，Then 搜索框保持交互且其余背景可拖动。Given backdrop 不支持，Then 实色回退且系统菜单/Snap 可用。

## 输入、焦点与跨窗口生命周期

拖动区只含非交互背景；Alt+Space、双击最大化、Snap Layout、键盘和触摸命中保持系统语义。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

Resize/DPI 时每帧最多一次区域重算，UI 工作小于 2 ms；重复 Attach/Detach 100 次后无事件或 backdrop controller 泄漏。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

