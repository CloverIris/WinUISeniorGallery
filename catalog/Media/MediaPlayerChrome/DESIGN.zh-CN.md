# MediaPlayerChrome 设计

## 视觉

默认样式为低干扰的 Fluent 控制层：底部渐变衬底、44×44 epx 最小点击目标、主播放按钮具有最高层级。`Full` 展示全部操作，`Compact` 把低频操作放入溢出菜单，`Minimal` 只保留播放、时间和入口按钮。

## 交互

单击视频表面显示或隐藏控制层由宿主决定；控件自身只处理其视觉树内输入。Space/游戏手柄 A 切换播放，左右方向键跳转，J/L 后退/前进，M 静音，F 请求全屏，Esc 请求退出全屏。快捷键在文本输入或菜单焦点中不得截获。

## 响应式

- 宽度 ≥ 720 epx：`Full`。
- 360–719 epx：`Compact`。
- < 360 epx：`Minimal`。

显式 `DisplayMode` 优先；模板不得依赖固定窗口宽度。紧凑浮窗中必须保留播放/暂停、进度、音量入口和返回 Inline 请求。

## 主题与资源

只使用 `ThemeResource`。稳定资源键：`MediaPlayerChromeBackgroundBrush`、`MediaPlayerChromeForegroundBrush`、`MediaPlayerChromeSecondaryForegroundBrush`、`MediaPlayerChromeControlFillBrush`、`MediaPlayerChromeErrorBrush`、`MediaPlayerChromeCornerRadius`、`MediaPlayerChromeControlSpacing`、`MediaPlayerChromeAutoHideDuration`。

Light、Dark、High Contrast 必须等价可读；背景视频导致对比度不足时使用渐变遮罩而非硬编码黑色。

## 动效

控制层使用 160 ms 淡入、200 ms 淡出和轻微纵向位移。缓冲仅用不阻塞的进度动画。系统 Reduced Motion 开启时取消位移和缩放，只保留即时可见性变化与必要进度反馈。

## 输入与无障碍

支持鼠标、触摸、笔、键盘、触控板和游戏手柄。顺序为播放、停止、时间轴、音量、倍速、窗口模式、更多。按钮具有本地化名称；播放按钮名称反映下一动作。缓冲不反复公告；错误和模式变化作为适度优先级 LiveRegion 公告。
