# Snap Layouts 研究规格

## 历史原型结构

Windows 11 自 2021 首发版把 Snap Layouts 放入最大化按钮悬停/Win+Z 入口，以若干分区图示让用户选择窗口落位，并把已贴靠窗口组成 Snap Group。 系统入口包含 MaximizeButton flyout、LayoutTemplates、Zone hit regions、当前窗口预览和后续窗口填充分组；它属于 Shell 非客户区。

## 历史状态与焦点

```text
Hidden --> Triggered(hover/Win+Z) --> LayoutChoosing\nLayoutChoosing --> ZoneFocused --> Committing --> Hidden\nCommitting --> GroupFilling --> Hidden\nAny --> Canceled/Unavailable
```

研究页演示指针悬停、点击、方向键和 Enter，但不注册 Win+Z；Esc 取消预览并恢复拖拽源焦点。

## 保留的 Design DNA

布局选择在操作点附近、以图形而非比例数字表达、键盘与指针同等、先预览后提交、常见多任务布局可快速发现。

## 现代化与废弃边界

现代 Demo 明确抛弃模拟最大化按钮 flyout、移动真实 OS 窗口、创建 Snap Group、覆盖系统热区和使用 Windows 商标图形；只提炼应用内 Dock 预览。

## 现代 feature owner

Owner 登记：`windowing.dock-layout-preview`（`DockLayoutPreview`）。

DockLayoutPreview 是唯一现代 owner，仅管理应用内部面板投放；系统 Snap 仍由 Windows/Shell 管理。 考古依赖只能从 Gallery/Archaeology 指向 owner，稳定模块不得反向引用展项。

## Gallery 研究树

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (DockLayoutPreview)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## 失败与晋级闭锁

owner/能力/数据缺失时保留静态研究，不伪造系统行为。进入 specified 前锁定来源、owner、资产许可、差异、自动化语义和平台禁区。
