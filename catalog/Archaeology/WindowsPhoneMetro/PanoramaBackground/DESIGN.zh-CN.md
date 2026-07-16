# Panorama Background 现代化设计

## 原型视觉与保留项

裁剪的 BackgroundLayer、可选 Tint/Scrim、PanoramaTitle 和独立 ForegroundStrip；背景不参与命中测试。 保留：共享背景连接 Section、受控视差表达深度、内容运动驱动背景而非独立动画。

## 抛弃项

抛弃为特定手机裁剪的单张超宽图、文字直接压在高频背景、晕动过强和背景预载阻塞；支持焦点裁剪、遮罩与静态回退。

## 输入与焦点

背景从 Hub 滚动进度被动更新，不响应指针；键盘、手柄、触摸得到相同位移。Reduced Motion 固定背景。 焦点视觉使用现代系统样式，任何自动动画不得抢焦点或改变 Automation 名称。

## 响应式与本地化

按 FocalPoint cover 裁剪，不拉伸；超宽窗口降低视差系数，竖窄屏保留背景语义区域，High Contrast 用实色。 中文、英文、长文本和 RTL 使用真实资源测试，不假设 Segoe/英语字宽。

## 主题、动效与无障碍

Modern Demo 只用 ThemeResource。High Contrast 不依赖图片/透明度，Reduced Motion 去除视差/缩放/滑动但保留状态。触控目标至少 44×44 epx。

## 资产禁区

不得提交产品截图、封面、艺术家图、商标、声音、原字体文件或从安装包提取资源；只用原创几何、许可兼容素材和文字结构图。

