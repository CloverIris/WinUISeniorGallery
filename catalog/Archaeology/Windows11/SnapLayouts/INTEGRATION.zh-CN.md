# Snap Layouts 集成

## Owner 与依赖边界

现代 owner：windowing.dock-layout-preview（DockLayoutPreview）。DockLayoutPreview 是唯一现代 owner，仅管理应用内部面板投放；系统 Snap 仍由 Windows/Shell 管理。 展项不复制 owner API/资源键，稳定模块不依赖 Archaeology。

## Gallery 数据与平台禁区

结构/DNA/差异来自本地 Markdown，Demo 用匿名确定性假数据；不得访问 Shell、任务栏、Start、真实固定项、账号、麦克风或全局输入。

## 窗口、输入与生命周期

Demo 只在 Gallery 当前 XamlRoot。研究页演示指针悬停、点击、方向键和 Enter，但不注册 Win+Z；Esc 取消预览并恢复拖拽源焦点。 页面卸载取消动画/Provider/计时，忽略迟到结果并恢复导航焦点。

## 降级与自动化

owner 或效果缺失时保留结构图、状态图和 Sources；High Contrast/Reduced Motion/屏幕阅读器必须有静态等价路径。

## 版权与隐私

license_review 保持 pending；资产记录作者/许可/来源。不收集查询、固定项、任务、语音或交互遥测。

