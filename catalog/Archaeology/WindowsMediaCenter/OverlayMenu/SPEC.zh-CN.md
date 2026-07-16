# Media Center Overlay Menu 研究规格

## 历史原型结构

Windows Media Center 播放期间通过遥控器呼出半透明覆盖菜单，在不中断视频画面的前提下显示播放、字幕、音轨、录制或设置。 VideoSurface 上叠 Scrim、OverlayPanel、PrimaryCommands、NestedSettings 和状态/时间；关闭后完整恢复视频上下文。

## 历史交互状态

```text
Hidden --> Opening --> Root\nRoot <--> Nested\nRoot/Nested --> Closing --> Hidden\nAny --> Disabled/Error
```

## 保留的 Design DNA

不离开播放上下文、短路径遥控操作、半透明保留视频感知、层级返回、自动隐藏但用户交互时暂停计时。

## 明确抛弃与现代化

抛弃透明度导致低对比、自动隐藏抢焦点、遥控器唯一入口和覆盖层自行暂停媒体；采用遮罩、焦点保持、多输入与宿主播放策略。

## 现代 owner 与 API 边界

OverlayMenu 拥有菜单层级/焦点；展项只提供 Media Center 命令排列和历史说明，不拥有播放器。 依赖仅从 Archaeology/Gallery 指向 controls.overlay-menu；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
ExhibitPage
|- Header (origin, era, proposed/pending)
|- PrototypeStructureDiagram (no original assets)
|- DesignDnaAndTradeoffs
|- ModernDemo (OverlayMenu)
|- InputAccessibilityMatrix
`- SourcesAndCopyright
```

## 失败与闭锁条件

owner/效果/数据缺失时显示静态结构和原因，不伪造历史产品。进入 specified 前完成来源、资产许可、状态和现代差异评审。

