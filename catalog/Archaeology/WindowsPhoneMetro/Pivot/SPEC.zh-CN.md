# Pivot 研究规格

## 历史原型结构

Windows Phone 7/8 的轻量顶层导航：超出屏幕的文字标题横排，当前标题与内容同步平移，左右轻扫连续切换相邻页。 视觉由 HeaderStrip、SelectedHeader、相邻标题暗示和单个 ContentViewport 构成；不同时实例化所有页面。

## 历史交互状态

```text
Idle --> DragTracking --> Settling --> Selected\nSelected --> HeaderInvoked --> Settling\nAny --> Disabled/Unloaded
```

## 保留的 Design DNA

轻量导航、相邻内容可发现、标题宽度随文字变化、内容与标题共享运动进度。

## 明确抛弃与现代化

抛弃只能靠横向手势、无限循环默认、超大标题裁切和整页预加载；增加显式点击、焦点、溢出和虚拟化。

## 现代 owner 与 API 边界

由 PivotView 稳定实现；考古页仅解释 WP 结构并配置同一现代控件展示触摸/键盘差异。 依赖方向只能从 Archaeology/Gallery 指向 controls.pivot-view；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (PivotView)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## 失败与闭锁条件

缺少现代 owner、演示数据或效果能力时显示静态结构与原因，不伪造原产品。进入 specified 前须完成来源复核、资产许可、原型状态和现代差异评审。

