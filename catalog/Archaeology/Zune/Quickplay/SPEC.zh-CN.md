# Zune Quickplay 研究规格

## 历史原型结构

Zune HD/软件 Quickplay 把 pinned、recent、history、new 等个人入口组成滚动首页，以超大类别文字和内容缩略快速回到常用媒体。 QuickplayHeader、固定类别 Section、每 Section 的预览项/计数、Pin 管理与最近历史；它是首页信息架构而非单一控件。

## 历史交互状态

```text
LoadingSections --> Ready\nReady --> SectionFocused/ItemInvoked\nReady --> Pinning/Reordering --> Ready\nAny --> EmptySection/PartialError
```

## 保留的 Design DNA

以个人活动而非媒体类型开始、固定与最近并列、少量高价值入口、超大标题作为导航地标。

## 明确抛弃与现代化

抛弃固定类别/硬编码数据源、隐私不透明历史和仅触摸重排；支持用户关闭历史、Provider 分区、键盘重排和空状态。

## 现代 owner 与 API 边界

HomeScreen 拥有 Section 组合/数据；展项提供 Quickplay 布局示例，不拥有 Pin/History 服务 API。 依赖方向只能从 Archaeology/Gallery 指向 experiences.home-screen；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (HomeScreen)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## 失败与闭锁条件

缺少现代 owner、演示数据或效果能力时显示静态结构与原因，不伪造原产品。进入 specified 前须完成来源复核、资产许可、原型状态和现代差异评审。

