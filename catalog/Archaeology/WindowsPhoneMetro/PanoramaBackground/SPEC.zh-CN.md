# Panorama Background 研究规格

## 历史原型结构

Windows Phone 7 Panorama 使用超宽图像跨多个内容面，前景移动距离大于背景，形成低成本深度与连续世界感。 裁剪的 BackgroundLayer、可选 Tint/Scrim、PanoramaTitle 和独立 ForegroundStrip；背景不参与命中测试。

## 历史交互状态

```text
Idle --> Panning(progress) --> Settling\nTheme/AssetChanged --> RecomputingCrop --> Idle\nAny --> StaticFallback
```

## 保留的 Design DNA

共享背景连接 Section、受控视差表达深度、内容运动驱动背景而非独立动画。

## 明确抛弃与现代化

抛弃为特定手机裁剪的单张超宽图、文字直接压在高频背景、晕动过强和背景预载阻塞；支持焦点裁剪、遮罩与静态回退。

## 现代 owner 与 API 边界

HubPanorama 拥有背景/视差配置；展项仅说明历史数学关系并使用自有抽象图形，不提供新 API。 依赖方向只能从 Archaeology/Gallery 指向 experiences.hub-panorama；展项不声明类型、资源键、服务或平台能力。

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

