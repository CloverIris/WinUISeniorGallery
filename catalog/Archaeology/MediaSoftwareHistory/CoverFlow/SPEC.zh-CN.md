# Cover Flow 研究规格

## 历史原型结构

Cover Flow 并非 Zune 原创。Apple 于 2006-09-12 在 iTunes 7 中把 Cover Flow 列为新功能：中央封面正对用户，邻近封面以透视角排列，滚轮/拖动/键盘改变中心项。 CenterCover、左右 PerspectiveNeighbors、Reflection/Shadow、水平索引与当前媒体信息构成 3D 风格浏览器；中心项是唯一主要调用目标。

## 历史交互状态

```text
Idle --> Dragging/Wheeling --> InertialSettling --> Centered\nCentered --> ItemInvoked\nAny --> FlatFallback/Unloaded
```

## 保留的 Design DNA

中心项层级极强、邻项表达序列、滚动与透视连续、封面浏览具有物理感和快速扫视能力。

## 明确抛弃与现代化

抛弃倒影作为必需、过度 3D/晕动、全量封面纹理、只支持横向 LTR 和把它归入 Zune；使用虚拟化、可关效果、Reduced Motion 平面模式和 RTL。

## 现代 owner 与 API 边界

CarouselView 拥有稳定轮播/虚拟化/输入 API；展项只提供 CoverFlow transition preset 和媒体软件史研究，不复制 Apple UI。 依赖仅从 Archaeology/Gallery 指向 controls.carousel-view；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
ExhibitPage
|- Header (origin, era, proposed/pending)
|- PrototypeStructureDiagram (no original assets)
|- DesignDnaAndTradeoffs
|- ModernDemo (CarouselView)
|- InputAccessibilityMatrix
`- SourcesAndCopyright
```

## 失败与闭锁条件

owner/效果/数据缺失时显示静态结构和原因，不伪造历史产品。进入 specified 前完成来源、资产许可、状态和现代差异评审。

