# Widgets Board 研究规格

## 历史原型结构

Windows 11 Widgets Board 从任务栏打开侧边面板，组合可调整大小的小组件卡片、个性化信息流、搜索/账户入口和滚动内容。 BoardChrome、WidgetGrid、Small/Medium/Large cards、拖拽重排、更多菜单、刷新状态和 Feed 区组成分层面板。

## 历史状态与焦点

```text
Closed --> Opening --> LoadingBoard --> Ready\nReady --> Reordering/Resizing/Refreshing --> Ready\nReady --> WidgetError/FeedPartial\nAny --> Closing --> Closed
```

键盘/手柄在卡片间二维导航，菜单键打开操作，拖拽有键盘替代；关闭 Board 恢复入口焦点，卡片刷新不抢焦点。

## 保留的 Design DNA

一眼可读的动态卡片、统一尺寸档、用户固定/重排、局部刷新与独立错误、侧边快速入口不打断主任务。

## 现代化与废弃边界

抛弃系统任务栏入口、个性化新闻 Feed、账号跟踪、后台无限刷新、网页小组件和系统面板外观；现代化为应用拥有的 Dashboard。

## 现代 feature owner

Owner 登记：`experiences.widgets-board`（`WidgetsBoard`）。

WidgetsBoard 拥有现代卡片布局/生命周期；考古页只配置本地匿名 WidgetProvider 和 Windows 11-inspired 信息架构。 考古依赖只能从 Gallery/Archaeology 指向 owner，稳定模块不得反向引用展项。

## Gallery 研究树

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (WidgetsBoard)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## 失败与晋级闭锁

owner/能力/数据缺失时保留静态研究，不伪造系统行为。进入 specified 前锁定来源、owner、资产许可、差异、自动化语义和平台禁区。
