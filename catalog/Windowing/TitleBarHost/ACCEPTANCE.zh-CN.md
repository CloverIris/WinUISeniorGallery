# TitleBarHost 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成并双语同步后才可 ready。

## Given / When / Then

Given RightContent 有三个按钮，When RTL 和 200% 文本缩放，Then 按钮仍可聚焦、区域不重叠系统按钮且背景可拖动。

## 输入、焦点与跨窗口生命周期

Tab 只进入交互内容，不进入拖动背景；系统按钮顺序、RTL、Alt+Space 和双击标题栏均保留。 覆盖 Closed、卸载、取消、DPI/显示器/主题变化及宿主拒绝；完成后无焦点陷阱、迟到事件或窗口引用泄漏。

## 性能预算

布局变更合并到每帧一次 RegionsChanged，测量小于 2 ms；标题高频更新不重建视觉树。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/回滚；UI 测 Light/Dark/High Contrast、Reduced Motion、键鼠触笔/手柄、Narrator、RTL、100%–300% DPI、200% 文本和多窗口关闭顺序。

## Ready 晋级验收

确定性假 Window/AppWindow、虚拟时钟和失败注入全部通过，平台降级/Automation/性能可重复；否则仍 proposed。

