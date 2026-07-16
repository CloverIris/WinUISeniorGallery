# Zune Big Typography 研究规格

## 历史原型结构

Zune 与 Metro 使用超大、轻字重、常为小写的 Segoe 标题作为空间与品牌元素；滚动时标题可部分离屏，让内容取代传统窗口框。 BigTitle、可选 Eyebrow/Subtitle、内容起始基线和滚动缩小/固定后的 CompactTitle；排版本身定义层级。

## 历史交互状态

```text
Expanded --> Collapsing(progress) --> Compact\nCompact --> Expanding --> Expanded\nAny --> Wrapped/ClampedFallback
```

## 保留的 Design DNA

大胆字号、留白、轻字重、内容优先、滚动中标题连续变化而非突然替换。

## 明确抛弃与现代化

抛弃固定 72pt、英文小写假设、标题裁切、对 CJK/RTL 不适配和轻字重低对比；使用语言感知字号/字重与文本缩放。

## 现代 owner 与 API 边界

BigTitle 拥有排版与滚动状态；展项仅提供 Zune preset 与设计解释，不新增字体或标题 API。 依赖方向只能从 Archaeology/Gallery 指向 controls.big-title；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (BigTitle)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## 失败与闭锁条件

缺少现代 owner、演示数据或效果能力时显示静态结构与原因，不伪造原产品。进入 specified 前须完成来源复核、资产许可、原型状态和现代差异评审。

