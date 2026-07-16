# Hub 研究规格

## 历史原型结构

Windows Phone 8.1 Hub 把异构 Section 放在一个横向全景画布中，Section 可有不同模板，标题与背景以不同速度移动。 HubHeader、横向 SectionStrip、可变宽 Section、SectionHeader、背景层和语义跳转入口构成连续内容面。

## 历史交互状态

```text
Loading --> Ready --> Panning --> Settling --> Ready\nReady --> SectionInvoked/NestedScroll\nAny --> PartialError/Unloaded
```

## 保留的 Design DNA

异构内容共存、横向探索、Section 级标题、全局背景营造连续空间、首屏突出编辑精选。

## 明确抛弃与现代化

抛弃无限宽预加载、背景文字对比不可控、嵌套横向滚动冲突和手势唯一入口；改用 Section 虚拟化、遮罩和显式跳转。

## 现代 owner 与 API 边界

HubPanorama 拥有实现与布局；考古页提供结构图、无版权假内容和可调视差，不拥有 Section API。 依赖方向只能从 Archaeology/Gallery 指向 experiences.hub-panorama；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (HubPanorama)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## 失败与闭锁条件

缺少现代 owner、演示数据或效果能力时显示静态结构与原因，不伪造原产品。进入 specified 前须完成来源复核、资产许可、原型状态和现代差异评审。

