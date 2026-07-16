# Focus Sessions Modern Design

## Prototype Structure and Design DNA

DurationPicker, focus/break cycles, TimerRing, TaskPicker, DailyProgress, AudioProvider, and Start/Pause/End controls form the session panel. Preserve: Time tied to task, focus/break rhythm, visible progress, optional music that never blocks timer, and end summary.

## Explicitly Discarded

Discard mandatory Microsoft account, hard Spotify/To Do dependency, automatic system Do Not Disturb control, and cloud tracking; core timer works locally with explicit Provider consent.

## Input and Focus

Space/A starts/pauses, End confirms, and keyboard/touch/gamepad work; announce minute/stage changes, never every second to Narrator. Automatic updates never steal focus or rename Automation; closing an overlay restores an explicit trigger.

## Responsive, RTL, and Automation

Narrow stacks and wide splits timer/tasks; minimized timing continues through host clock and resumes by absolute time. RTL does not reverse time flow. At 200% text, status/exit never clips; Automation exposes role, state, position, and next action.

## Theme and High Contrast

Use ThemeResource only. High Contrast does not depend on Acrylic/Mica/shadow/transparency; Reduced Motion removes slide/scale but preserves state.

## Copyright and Asset Exclusion Zone

Commit no Windows 11 screenshot, system icon, wallpaper, sound, font, news content, brand text, or extracted package asset; use owned geometry/fake data.

