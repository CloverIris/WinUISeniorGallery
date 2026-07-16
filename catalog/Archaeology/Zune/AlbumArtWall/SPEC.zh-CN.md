# Zune Album Art Wall 研究规格

## 历史原型结构

Zune 软件在浏览与播放背景中使用密集专辑封面墙，把音乐库视觉化为连续马赛克，并让当前专辑或艺术家在图像海洋中突出。 规则/半规则 CoverGrid、裁剪封面、选中强调层与可选暗化背景；文字信息置于独立前景。

## 历史交互状态

```text
Empty --> LoadingThumbnails --> Ready\nReady --> Focus/SelectionChanged --> Ready\nAny --> PartialImages/StaticFallback
```

## 保留的 Design DNA

内容本身成为品牌背景、重复网格形成节奏、当前项通过尺度/明度而非重边框突出。

## 明确抛弃与现代化

抛弃一次加载全库、随机抖动布局、低对比文字和无来源图片；改用虚拟化、稳定 ID、遮罩、占位和许可素材。

## 现代 owner 与 API 边界

AdaptiveGrid 拥有网格/虚拟化；展项组合它展示 Zune DNA，不拥有封面加载或选择 API。 依赖方向只能从 Archaeology/Gallery 指向 controls.adaptive-grid；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (AdaptiveGrid)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## 失败与闭锁条件

缺少现代 owner、演示数据或效果能力时显示静态结构与原因，不伪造原产品。进入 specified 前须完成来源复核、资产许可、原型状态和现代差异评审。

