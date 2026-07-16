# Snapped View 研究规格

## 历史模型

Windows 8 将应用分为主区与窄 Snap 区；宽度变化会改变可显示信息和应用状态。研究视觉树为 `Workspace → LayoutHint → PrimaryRegion + SnapRegion → RegionContent`，讲解状态为 `Single`、`Previewing`、`Snapped`、`ReflowRequired`、`Unavailable`。这是历史描述，不是窗口尺寸或 dock 算法的 API 合同。

## 输入和失败边界

历史操作来自拖到边缘、触摸拖动和系统手势；Gallery 只提供按钮、键盘和模拟拖拽。区域太窄时不缩小到不可读：显示 reflow 注解或堆叠示意。没有多窗口能力、拖拽取消、宿主拒绝或动画禁用时，示例恢复为静态布局，不创建/移动任何窗口。

## 非目标

不劫持系统 Snap、不保证 1/3 或 1/2 比率、不保存布局、不制作 `PART_*` 或公开命令。现代 `DockLayoutPreview` 决定真实预览、窗口边界和失败语义。
