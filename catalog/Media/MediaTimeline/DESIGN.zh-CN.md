# MediaTimeline 设计

## 视觉

轨道默认高 4 epx，交互时扩展至 6 epx；Thumb 目标区域至少 44×44 epx。播放进度、缓冲、禁用区间、章节和普通标记使用层级清晰的 ThemeResource，不仅依赖颜色区分。章节使用短刻度，标记使用可选图标，禁用区间带纹理或高对比边界。

## 交互

鼠标悬停和触摸拖动显示格式化时间；缩略图由宿主通过 ToolTip 内容模板提供。拖动时播放位置不跳动，预览层跟随指针。点击轨道直接提交一次 Seek。非 1.0 倍速时可显示紧凑倍速标签，但媒体刻度始终保持原始时间。Live DVR 离开 Live Edge 后显示“回到直播”入口。

## 响应式

宽度不足 240 epx 时隐藏章节标签与当前/总时间文字，只保留轨道和 Live 状态。紧凑模式仍保留完整键盘与 Automation 操作。标记密集时按像素聚合，不改变底层集合。

## 主题与资源

稳定资源键：`MediaTimelineTrackBrush`、`MediaTimelineProgressBrush`、`MediaTimelineBufferedBrush`、`MediaTimelineDisabledBrush`、`MediaTimelineThumbBrush`、`MediaTimelineMarkerBrush`、`MediaTimelineChapterBrush`、`MediaTimelineLiveBrush`、`MediaTimelineTrackHeight`、`MediaTimelineExpandedTrackHeight`、`MediaTimelineThumbSize`。

## 动效

Thumb 与轨道展开可使用 120 ms 状态过渡；位置更新不得使用造成滞后的缓动。Reduced Motion 下即时切换尺寸和标记状态。高频进度变化优先更新渲染属性，避免重建视觉树。

## 输入与无障碍

左右方向键使用 `KeyboardStep`，PageUp/PageDown 使用 `LargeKeyboardStep`，Home 到最早有效位置，End 到末尾或 Live Edge。RTL 的视觉方向镜像，键盘“左/右”遵循视觉方向。AutomationPeer 实现 RangeValue；Live 模式为只读，值文本包含直播、已落后时间或墙钟时间。
