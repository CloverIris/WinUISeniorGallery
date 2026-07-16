# MediaTimeline Design

## Visuals

The track defaults to 4 epx and expands to 6 epx during interaction; the Thumb target is at least 44×44 epx. Progress, buffering, disabled ranges, chapters, and markers use hierarchical ThemeResources and are not distinguished by color alone. Chapters use ticks, markers use optional icons, and disabled ranges use texture or high-contrast boundaries.

## Interaction

Mouse hover and touch drag show formatted time; the host supplies thumbnails through the ToolTip content template. While scrubbing, playback position remains fixed and the preview layer follows the pointer. Clicking the track commits one Seek. A compact rate label may appear away from 1.0×, while ticks remain in original media time. Leaving Live Edge in Live DVR reveals a Go Live action.

## Responsive Behavior

Below 240 epx, hide chapter labels and current/total time text, retaining track and live state. Compact mode keeps complete keyboard and Automation behavior. Dense markers are pixel-clustered without changing the source collection.

## Theme and Resources

Stable keys are `MediaTimelineTrackBrush`, `MediaTimelineProgressBrush`, `MediaTimelineBufferedBrush`, `MediaTimelineDisabledBrush`, `MediaTimelineThumbBrush`, `MediaTimelineMarkerBrush`, `MediaTimelineChapterBrush`, `MediaTimelineLiveBrush`, `MediaTimelineTrackHeight`, `MediaTimelineExpandedTrackHeight`, and `MediaTimelineThumbSize`.

## Motion

Thumb and track expansion may use a 120 ms state transition. Position updates never use lagging easing. Reduced Motion switches sizes and marker states immediately. High-frequency progress updates prefer render properties and avoid rebuilding the visual tree.

## Input and Accessibility

Arrow keys use `KeyboardStep`, PageUp/PageDown use `LargeKeyboardStep`, Home uses the earliest valid position, and End uses the end or Live Edge. RTL mirrors visual direction and Left/Right follow visual direction. The AutomationPeer implements RangeValue; Live is read-only and value text conveys live state, latency, or wall-clock time.
