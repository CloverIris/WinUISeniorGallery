# DockLayoutPreview 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given 三个目标，When 指针进入无效区域，Then 预览消失且原布局不变。Given commit 抛错，Then 原面板位置/焦点原子恢复。

## 输入、焦点与跨窗口生命周期

鼠标/触摸/笔拖拽与键盘 Dock 命令等价；屏幕阅读器可枚举目标并确认，不要求精确指针。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

拖动 60 Hz，命中测试每帧小于 2 ms；目标几何仅布局版本改变时重建，关闭覆盖层零命中。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

