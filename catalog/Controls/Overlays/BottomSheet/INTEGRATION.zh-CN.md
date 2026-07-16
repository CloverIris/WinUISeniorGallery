# BottomSheet 集成

## 宿主与 Contract

每个窗口内容根部可放置一个或多个 BottomSheet，但同一 Z 层只允许一个处于 Open/Opening；后打开者位于前者上方并成为 Esc 目标。使用 Theme、Motion、Input、Accessibility Contract 和窗口级 Overlay 协调器。控件不创建 `AppWindow`。

所有调用在 UI 线程执行。应用应把 Sheet 放在覆盖完整客户区的 Grid 最后层，避免在 ScrollViewer 或裁剪容器内。卸载时强制清理，不将状态迁移到其他窗口。

## 平台、生命周期和降级

位移使用 `Microsoft.UI.Composition`，输入使用 Pointer Capture，焦点使用 WinUI FocusManager。无网络、文件、权限或外部 API。窗口停用时保留 Open 状态但停止动画、释放指针；再次激活重新测量。

Composition 不可用或系统动画关闭时直接应用最终布局。无法建立焦点约束时模态打开失败并抛 `InvalidOperationException`，避免产生视觉模态但背景仍可操作的危险状态。
