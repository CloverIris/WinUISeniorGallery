# Charms Bar 研究规格

## 研究目标与非目标

目标是还原其“上下文命令在需要时出现、主内容不离场”的信息架构：右侧纵向带、五个语义入口、按当前应用/内容改变可用性。非目标包括注册全局热角、截获 `Win+C`、访问系统 Share/Devices/Start，或把历史名词写成公开控件 API。

## 历史结构与状态

历史视觉树可概括为 `Screen → EdgeAffordance → CharmRail → CharmItem → ContextPane`。研究展示的状态为 `Closed`、`Hinted`、`RailVisible`、`ContextPaneVisible` 和 `Unavailable`；仅用于讲解。每项有图标、文本、可用性和说明，Settings 可打开应用内二级面板，Share/Devices 取决于上下文。

## 输入、失败与降级

历史入口包含右边缘滑入、热角、鼠标和 `Win+C`；Gallery 只允许按钮、键盘焦点和模拟滑入。触摸或动画不可用时直接显示静态 rail；窄窗、无上下文或宿主拒绝时展示“历史能力，当前不可执行”，不伪造系统调用。所列 `PART_*`、AutomationId 和命令均属于未来 `EdgeCommandPanel` 规格，而非本研究目录。
