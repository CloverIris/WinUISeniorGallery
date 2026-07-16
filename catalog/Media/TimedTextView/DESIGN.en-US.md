# TimedTextView Design

## Visuals

SingleLine uses a centered caption surface of at most two lines; ScrollingLyrics centers the active row with context; Karaoke fills the foreground for elapsed words; Bilingual places original above translation. Interim ASR uses a subtle opacity difference and optional state glyph, never continuous flashing.

## Interaction

Clicking or pressing Enter on a Segment only raises `SegmentInvoked`. Wheel, touch, and keyboard scrolling suspend auto-centering; a Return to Current action restores it. Text selection is off by default to avoid drag conflicts, while a transcript template may enable selection.

## Responsive Behavior

- < 360 epx: reduce context and margins; Bilingual allows at most two lines per language.
- 360–719 epx: show standard captions or five lines of lyric context.
- ≥ 720 epx: allow a wider lyric column with 960 epx maximum body width.

At 200% text scaling, content is not clipped. When height is constrained, reduce context instead of font size.

## Theme and Resources

Stable keys are `TimedTextPrimaryForegroundBrush`, `TimedTextSecondaryForegroundBrush`, `TimedTextInactiveForegroundBrush`, `TimedTextHighlightBrush`, `TimedTextInterimForegroundBrush`, `TimedTextCaptionBackgroundBrush`, `TimedTextCornerRadius`, `TimedTextLineSpacing`, `TimedTextScrollDuration`, and `TimedTextMaxTextWidth`.

All colors come from ThemeResource. High Contrast uses system text, background, and highlight resources. Original and translation differ by position and text style in addition to color.

## Motion

Auto-scroll uses position transitions up to 220 ms and does not start an animation for every playback update. Karaoke word fill may use clipping or Composition properties. Reduced Motion positions the active line and changes word state immediately while retaining understandable text updates.

## Input and Accessibility

Support mouse, touch, pen, keyboard, touchpad, and gamepad scrolling. Every invokable Segment uses ListItem/Invoke semantics and exposes start time, language, and Final/Interim state. Active Segment changes use a polite LiveRegion; Karaoke Words are not announced individually. Mixed LTR/RTL text follows each Track language.
