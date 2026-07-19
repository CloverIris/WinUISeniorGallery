# MediaTimeline Acceptance

## Functional Scenarios

- Given VOD `[0, 120s]`, when Position is 30s, then progress and RangeValue are both 25%.
- Given Live mode, when the track is clicked, then no Seek is raised and “Live” is announced.
- Given a Live DVR window moving from `[100s, 400s]` to `[110s, 410s]`, when not scrubbing, then Live Edge and relative position do not flash-jump.
- Given a one-second drag with 100 ms throttling, when pointer events are frequent, then preview occurs no more than 11 times, includes the final preview, and release raises one Seek.
- Given a canceled drag, then no Seek is raised and host `Position` is restored.
- Given a target in a disabled range, then nearest boundary and drag direction produce one deterministic valid position.
- Given End in Live DVR, then one Live Edge request is raised; if unhandled, Seek the current `Maximum`.
- Given `PlaybackRate=2.0`, then presentation and Automation description include 2.0× while media range, stepping, and Seek positions remain unscaled.

## Ranges and Boundaries

- Clip, sort, and merge overlapping buffered/disabled ranges; ignore zero-length, reversed, and out-of-range data.
- `Maximum < Minimum`, empty collections, and dynamic replacement never throw.
- When the window moves during scrub, release is reclamped against the latest range.
- Chapters and markers at one position retain input order; visual clustering loses no Automation information.

## Performance

- Initial layout of 1,000 markers on target Release x64 hardware does not block the UI beyond one frame budget for three consecutive frames.
- Frequent `Position` updates do not rebuild range/marker visuals; unchanged collections use a reference-equality fast path.
- A 60-second scrub shows no sustained memory growth and no timer callbacks after unload.

## Input and Accessibility

- Verify mouse click/drag, touch, pen, keyboard, and gamepad stepping.
- Verify Light, Dark, High Contrast, Reduced Motion, RTL, and 100%–300% DPI.
- RangeValue Minimum, Maximum, Value, SmallChange, and LargeChange are correct; read-only Live exposes no SetValue.

## Automation

Unit-test normalization, disabled-range correction, throttle trailing value, final commit, and Live Edge. UI-test template degradation, ToolTip, focus, RTL, and AutomationPeer. Use a virtual clock for deterministic tests.

## Implementation Evidence

- 2026-07-16: Media Release x64 and Gallery Debug/Release x64 builds succeeded; Media automation tests passed 9/9.
- Gallery provides deterministic VOD, Live, and Live DVR sessions with disabled-range, chapter, and marker demonstrations.
- Touch, pen, gamepad, RTL, and frame-budget checks remain manual review work; the item remains `in-progress`.
