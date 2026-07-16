# Live Tile 研究规格

## 历史原型结构

Windows Phone 开始屏幕的 Live Tile 以小/中/宽等尺寸显示应用身份与动态摘要，并通过翻转、循环、图标和计数在不打开应用时传递状态。 TileSurface 含品牌层、Primary/SecondaryContent、Badge 和 Size Variant；更新由系统调度，开始屏幕控制布局。

## 历史交互状态

```text
Static --> UpdateQueued --> Transitioning --> Live\nLive --> Stale/Paused --> Live\nAny --> Disabled
```

## 保留的 Design DNA

可扫视信息、固定尺寸变体、动态但短促的内容轮换、品牌与状态合一、用户决定布局。

## 明确抛弃与现代化

不模拟 OS 开始屏幕、后台 Tile API、无休止翻转或不可控通知；现代化为应用内 Dashboard 卡片，用户可暂停动态内容。

## 现代 owner 与 API 边界

DynamicTile 拥有稳定尺寸、内容和动画 API；展项只映射历史尺寸/状态并使用本地确定性数据。 依赖方向只能从 Archaeology/Gallery 指向 controls.dynamic-tile；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (DynamicTile)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## 失败与闭锁条件

缺少现代 owner、演示数据或效果能力时显示静态结构与原因，不伪造原产品。进入 specified 前须完成来源复核、资产许可、原型状态和现代差异评审。

