# Media Center Grid 研究规格

## 历史原型结构

Windows Media Center 的海报墙/媒体库网格为遥控器优化，方向键在二维卡片间移动，焦点项突出并可在同屏展示标题或详情。 GridViewport、虚拟化海报行列、FocusChrome、Page/SectionHeader 和 DetailsOverlay；选中与焦点可分离。

## 历史交互状态

```text
Empty --> Loading --> Ready\nReady --> FocusMoving/DetailsVisible --> Ready\nReady --> ItemInvoked\nAny --> PartialError/Unloaded
```

## 保留的 Design DNA

稳定二维空间、10-foot 焦点、内容优先海报、焦点移动驱动详情、返回路径清晰。

## 明确抛弃与现代化

抛弃固定列数、低分辨率位图、遥控器唯一输入、放大导致布局抖动和全量实现；使用响应列、许可图片、渲染变换和虚拟化。

## 现代 owner 与 API 边界

MediaCenterGrid 拥有稳定网格行为；考古展项只映射原始结构/10-foot preset，不声明网格 API。 依赖仅从 Archaeology/Gallery 指向 media.media-center-grid；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
ExhibitPage
|- Header (origin, era, proposed/pending)
|- PrototypeStructureDiagram (no original assets)
|- DesignDnaAndTradeoffs
|- ModernDemo (MediaCenterGrid)
|- InputAccessibilityMatrix
`- SourcesAndCopyright
```

## 失败与闭锁条件

owner/效果/数据缺失时显示静态结构和原因，不伪造历史产品。进入 specified 前完成来源、资产许可、状态和现代差异评审。

