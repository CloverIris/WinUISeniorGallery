# Zune Now Playing 现代化设计

## 原型视觉与保留项

AmbientBackground、Artist/AlbumVisual、Metadata、Progress/Transport overlay 与可选队列/歌词；内容层与控制层独立。 保留：沉浸内容优先、色彩从媒体派生、控件渐进披露、标题与图像共同讲述当前媒体。

## 抛弃项

抛弃网络艺术家图像默认下载、文字压图低对比、控件自动隐藏导致键盘丢焦和持续背景运动；改用许可 Provider、遮罩、焦点保持和 Reduced Motion。

## 输入与焦点

任意输入显示控制；键盘焦点/菜单/拖动时不隐藏。Space/A 播放，方向键时间轴，B/Esc 退出沉浸；背景不命中。 焦点视觉使用现代系统样式，任何自动动画不得抢焦点或改变 Automation 名称。

## 响应式与本地化

小窗转为普通播放器，宽屏保留环境留白；CompactOverlay 不复刻全屏背景。高对比使用实色，不从封面取色。 中文、英文、长文本和 RTL 使用真实资源测试，不假设 Segoe/英语字宽。

## 主题、动效与无障碍

Modern Demo 只用 ThemeResource。High Contrast 不依赖图片/透明度，Reduced Motion 去除视差/缩放/滑动但保留状态。触控目标至少 44×44 epx。

## 资产禁区

不得提交产品截图、封面、艺术家图、商标、声音、原字体文件或从安装包提取资源；只用原创几何、许可兼容素材和文字结构图。

