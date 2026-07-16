# Start Menu Pinned Grid 研究规格

## 历史原型结构

Windows 11 Start 的 Pinned 区使用居中、自适应应用图标网格，支持分页、拖拽重排，并在后续版本加入文件夹组合；Recommended 与 Pinned 分区。 StartSurface 内含 PinnedHeader、固定列 Grid、PageIndicator、FolderTile/FolderFlyout、AllApps 入口和 Recommended 分区。

## 历史状态与焦点

```text
Ready --> Reordering --> Ready\nReady --> FolderOpen --> Ready\nReady --> Paging --> Ready\nAny --> Empty/PartialIconError
```

方向键/手柄按网格移动，Enter/A 调用，菜单键管理，键盘提供移动/建文件夹替代；图标公开文本名而非只读图形。

## 保留的 Design DNA

高频入口固定、统一图标节奏、用户重排、文件夹压缩空间、分页保持可预测位置。

## 现代化与废弃边界

抛弃模拟 Start、启动系统应用、读取真实固定列表、Recommended/广告、系统文件夹拖放和 Windows 图标资产；只提炼自适应固定网格。

## 现代 feature owner

Owner 登记：`controls.adaptive-grid`（`AdaptiveGrid`）。

AdaptiveGrid 拥有布局/虚拟化；文件夹、重排事务和启动命令由宿主体验层负责，考古项不拥有这些 API。 考古依赖只能从 Gallery/Archaeology 指向 owner，稳定模块不得反向引用展项。

## Gallery 研究树

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (AdaptiveGrid)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## 失败与晋级闭锁

owner/能力/数据缺失时保留静态研究，不伪造系统行为。进入 specified 前锁定来源、owner、资产许可、差异、自动化语义和平台禁区。
