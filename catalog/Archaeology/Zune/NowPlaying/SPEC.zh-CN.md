# Zune Now Playing 研究规格

## 历史原型结构

Zune 软件的全屏 Now Playing 将艺术家图像、专辑封面、环境色和大字排版融为背景，播放控制弱化并在需要时出现。 AmbientBackground、Artist/AlbumVisual、Metadata、Progress/Transport overlay 与可选队列/歌词；内容层与控制层独立。

## 历史交互状态

```text
LoadingArt --> AmbientReady\nAmbientReady <--> ControlsVisible\nPlaying <--> Paused/Buffering\nAny --> MissingArtFallback/Error
```

## 保留的 Design DNA

沉浸内容优先、色彩从媒体派生、控件渐进披露、标题与图像共同讲述当前媒体。

## 明确抛弃与现代化

抛弃网络艺术家图像默认下载、文字压图低对比、控件自动隐藏导致键盘丢焦和持续背景运动；改用许可 Provider、遮罩、焦点保持和 Reduced Motion。

## 现代 owner 与 API 边界

ImmersiveNowPlaying 组合 MediaPlayerChrome/TimedText 等稳定组件；展项只提供 Zune-inspired 主题和研究说明。 依赖方向只能从 Archaeology/Gallery 指向 experiences.immersive-now-playing；展项不声明类型、资源键、服务或平台能力。

## Gallery 展示树

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (ImmersiveNowPlaying)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## 失败与闭锁条件

缺少现代 owner、演示数据或效果能力时显示静态结构与原因，不伪造原产品。进入 specified 前须完成来源复核、资产许可、原型状态和现代差异评审。

