# Zune Mixview 研究规格

## 历史原型结构

Zune 3.0 时代 Mixview 以中心艺术家/专辑为核心，把相关推荐、歌曲和关系内容作为不同大小方块向外展开，点击节点重排网络。 CenterNode、环形/放射 RelatedNodes、按关系强度变化的尺寸、连接语义和详情层；不是任意物理图模拟器。

## 历史交互状态

```text
Empty --> LoadingRelations --> Ready\nReady --> NodeInvoked --> Reflowing --> Ready\nReady --> Panning/Zooming\nAny --> PartialError
```

## 保留的 Design DNA

关系可视化、中心上下文明确、内容尺寸编码重要性、探索通过重新居中持续进行。

## 明确抛弃与现代化

抛弃无解释的随机布局、无限动画、颜色作为唯一关系、节点全量加载和不可键盘导航；使用确定性布局、图例、分页与列表替代视图。

## 现代 owner 与 API 边界

MixviewExperience 拥有关系模型/布局；展项只定义历史 preset、假数据和对比说明，不声明图 API。 依赖方向只能从 Archaeology/Gallery 指向 experiences.mixview；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (MixviewExperience)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## 失败与闭锁条件

缺少现代 owner、演示数据或效果能力时显示静态结构与原因，不伪造原产品。进入 specified 前须完成来源复核、资产许可、原型状态和现代差异评审。

