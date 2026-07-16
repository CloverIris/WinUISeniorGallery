# Media Center Overlay Menu 现代化设计

## 原型视觉与保留项

VideoSurface 上叠 Scrim、OverlayPanel、PrimaryCommands、NestedSettings 和状态/时间；关闭后完整恢复视频上下文。 保留：不离开播放上下文、短路径遥控操作、半透明保留视频感知、层级返回、自动隐藏但用户交互时暂停计时。

## 抛弃项

抛弃透明度导致低对比、自动隐藏抢焦点、遥控器唯一入口和覆盖层自行暂停媒体；采用遮罩、焦点保持、多输入与宿主播放策略。

## 输入与焦点

Menu/Enter/A 打开，D-pad 导航，B/Esc 逐层返回；鼠标/触摸/键盘等价。菜单、焦点或子弹层存在时不自动隐藏。 自动动画不得抢焦点或更改 Automation 名称。

## 响应式与本地化

窄屏底部 Sheet，宽屏中心/侧面 Overlay，10-foot 增大目标；CompactOverlay 只保留核心命令，不塞完整设置。 中文、英文、长文本和 RTL 用真实资源测试。

## 主题、动效与无障碍

Demo 只用 ThemeResource；High Contrast 不依赖图片/透明度，Reduced Motion 去除视差/缩放/滑动但保留状态；目标至少 44×44 epx。

## 资产禁区

禁止产品截图、海报/封面、商标、声音、原字体和安装包提取资源；只用原创几何、许可兼容素材和文字结构图。

