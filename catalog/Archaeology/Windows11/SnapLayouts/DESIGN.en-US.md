# Snap Layouts Modern Design

## Prototype Structure and Design DNA

The system entry contains MaximizeButton flyout, LayoutTemplates, zone hit regions, current-window preview, and subsequent group filling; it belongs to Shell non-client UI. Preserve: Layout choice near the action point, graphical rather than numeric ratios, keyboard/pointer parity, preview before commit, and discoverable common multitasking arrangements.

## Explicitly Discarded

Modern Demo explicitly discards imitating maximize flyout, moving real OS windows, creating Snap Groups, covering system zones, or using Windows trademark graphics; it extracts in-app Dock preview only.

## Input and Focus

Research page demonstrates pointer hover/click, arrows, and Enter but never registers Win+Z; Escape cancels preview and restores drag-source focus. Automatic updates never steal focus or rename Automation; closing an overlay restores an explicit trigger.

## Responsive, RTL, and Automation

Demo generates 2–6 zones for host content, fewer on narrow layouts; RTL mirrors zone order and DPI/display change recomputes in-app coordinates. At 200% text, status/exit never clips; Automation exposes role, state, position, and next action.

## Theme and High Contrast

Use ThemeResource only. High Contrast does not depend on Acrylic/Mica/shadow/transparency; Reduced Motion removes slide/scale but preserves state.

## Copyright and Asset Exclusion Zone

Commit no Windows 11 screenshot, system icon, wallpaper, sound, font, news content, brand text, or extracted package asset; use owned geometry/fake data.

