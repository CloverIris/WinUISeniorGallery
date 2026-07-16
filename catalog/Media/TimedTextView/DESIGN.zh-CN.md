# TimedTextView 设计

## 视觉

SingleLine 使用最多两行的居中字幕底板；ScrollingLyrics 以活动行为视觉中心并显示上下文；Karaoke 用前景填充表达已播放文字；Bilingual 以上方原文、下方译文呈现。Interim ASR 使用轻微透明度差异和可选状态字形，不使用持续闪烁。

## 交互

点击或按 Enter 激活 Segment 仅发 `SegmentInvoked`。滚轮、触摸和键盘滚动暂停自动居中；提供“回到当前行”按钮恢复。文本选择默认关闭以免与拖动冲突，宿主可通过模板为讲稿场景启用选择。

## 响应式

- < 360 epx：限制上下文与边距，Bilingual 每种语言最多两行。
- 360–719 epx：显示标准字幕或 5 行歌词上下文。
- ≥ 720 epx：允许更宽歌词列，但正文最大宽度 960 epx。

字体缩放至 200% 时不裁切；窗口高度不足时减少上下文而非缩小字体。

## 主题与资源

稳定资源键：`TimedTextPrimaryForegroundBrush`、`TimedTextSecondaryForegroundBrush`、`TimedTextInactiveForegroundBrush`、`TimedTextHighlightBrush`、`TimedTextInterimForegroundBrush`、`TimedTextCaptionBackgroundBrush`、`TimedTextCornerRadius`、`TimedTextLineSpacing`、`TimedTextScrollDuration`、`TimedTextMaxTextWidth`。

所有颜色来自 ThemeResource。High Contrast 使用系统文本、背景和高亮资源；原文与翻译除颜色外通过位置和文本样式区分。

## 动效

自动滚动使用最多 220 ms 的位置过渡，不对每次播放位置变化启动动画。Karaoke 逐词填充可用裁剪或 Composition 属性。Reduced Motion 下活动行即时定位、逐词状态即时切换，保留可理解的文本变化。

## 输入与无障碍

支持鼠标、触摸、笔、键盘、触控板和手柄滚动。每个可调用 Segment 使用 ListItem/Invoke 语义并公开开始时间、文本语言和 Final/Interim 状态。活动 Segment 变化使用适度 LiveRegion；Karaoke Word 不逐词公告，避免语音轰炸。混合 LTR/RTL 文本按各 Track 语言确定方向。
