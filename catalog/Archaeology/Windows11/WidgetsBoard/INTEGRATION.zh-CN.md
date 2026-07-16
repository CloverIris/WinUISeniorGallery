# Widgets Board 集成

## Owner 与依赖边界

现代 owner：experiences.widgets-board（WidgetsBoard）。WidgetsBoard 拥有现代卡片布局/生命周期；考古页只配置本地匿名 WidgetProvider 和 Windows 11-inspired 信息架构。 展项不复制 owner API/资源键，稳定模块不依赖 Archaeology。

## Gallery 数据与平台禁区

结构/DNA/差异来自本地 Markdown，Demo 用匿名确定性假数据；不得访问 Shell、任务栏、Start、真实固定项、账号、麦克风或全局输入。

## 窗口、输入与生命周期

Demo 只在 Gallery 当前 XamlRoot。键盘/手柄在卡片间二维导航，菜单键打开操作，拖拽有键盘替代；关闭 Board 恢复入口焦点，卡片刷新不抢焦点。 页面卸载取消动画/Provider/计时，忽略迟到结果并恢复导航焦点。

## 降级与自动化

owner 或效果缺失时保留结构图、状态图和 Sources；High Contrast/Reduced Motion/屏幕阅读器必须有静态等价路径。

## 版权与隐私

license_review 保持 pending；资产记录作者/许可/来源。不收集查询、固定项、任务、语音或交互遥测。

