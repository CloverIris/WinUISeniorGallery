# Application Bar 研究规格

## 历史原型结构

Windows Phone 7/8 Application Bar 固定在底部，最多四个圆形主命令；点击省略号向上展开文字标签和次级菜单。 PrimaryIconRow、EllipsisButton、PrimaryLabels、SecondaryMenu 与半透明 Backplate；折叠时仍保留主图标。

## 历史交互状态

```text
Collapsed --> Expanding --> Expanded\nExpanded --> CommandInvoked/Collapsing --> Collapsed\nAny --> Disabled/Hidden
```

## 保留的 Design DNA

拇指可达底部操作、四个主命令上限、渐进披露、图标展开后获得文字解释、内容保持上下文。

## 明确抛弃与现代化

抛弃仅靠省略号猜测、固定手机高度、图标无 Tooltip 和 Secondary 命令不可键盘访问；增加显式名称、溢出和焦点管理。

## 现代 owner 与 API 边界

ExpandableCommandBar 拥有命令模型与展开行为；展项只给出 WP 排列 preset 和历史说明。 依赖方向只能从 Archaeology/Gallery 指向 controls.expandable-command-bar；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (ExpandableCommandBar)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## 失败与闭锁条件

缺少现代 owner、演示数据或效果能力时显示静态结构与原因，不伪造原产品。进入 specified 前须完成来源复核、资产许可、原型状态和现代差异评审。

