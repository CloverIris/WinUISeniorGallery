# Media Center Horizontal Strip 研究规格

## 历史原型结构

Windows Media Center 在电视距离用横向内容条组织电影、录制、音乐和照片；焦点项滚到安全区并轻微放大，左右遥控器连续浏览。 SectionHeader、HorizontalViewport、均匀 ItemCards、FocusChrome、边缘预览和可选 DetailsPresenter 构成一条类别轨道。

## 历史交互状态

```text
Empty --> Loading --> Ready\nReady --> FocusMoving --> Settling --> Ready\nReady --> ItemInvoked\nAny --> PartialError/Unloaded
```

## 保留的 Design DNA

10-foot 可读、单轴焦点、边缘项目暗示更多内容、焦点项增强而非改变布局、类别条可纵向堆叠。

## 明确抛弃与现代化

抛弃遥控器唯一输入、固定电视分辨率、焦点放大遮挡邻项和一次加载整库；加入触摸/鼠标/键盘、虚拟化和安全区。

## 现代 owner 与 API 边界

ContentRail 拥有稳定列表/虚拟化/焦点 API；展项只提供 Media Center preset 和研究说明。 依赖仅从 Archaeology/Gallery 指向 controls.content-rail；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
ExhibitPage
|- Header (origin, era, proposed/pending)
|- PrototypeStructureDiagram (no original assets)
|- DesignDnaAndTradeoffs
|- ModernDemo (ContentRail)
|- InputAccessibilityMatrix
`- SourcesAndCopyright
```

## 失败与闭锁条件

owner/效果/数据缺失时显示静态结构和原因，不伪造历史产品。进入 specified 前完成来源、资产许可、状态和现代差异评审。

