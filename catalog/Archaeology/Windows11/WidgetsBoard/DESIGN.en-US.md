# Widgets Board Modern Design

## Prototype Structure and Design DNA

BoardChrome, WidgetGrid, Small/Medium/Large cards, drag reorder, More menu, refresh state, and Feed section form the layered surface. Preserve: Glanceable dynamic cards, standard size tiers, user pin/reorder, local refresh/error isolation, and a side entry that does not replace the primary task.

## Explicitly Discarded

Discard system taskbar entry, personalized news feed, account tracking, unbounded background refresh, web widgets, and system-board appearance; modernize as app-owned dashboard.

## Input and Focus

Keyboard/gamepad uses 2D card navigation, menu opens actions, and drag has keyboard alternative; close restores entry focus and refresh never steals focus. Automatic updates never steal focus or rename Automation; closing an overlay restores an explicit trigger.

## Responsive, RTL, and Automation

Narrow one column, regular two, wide three/four; 200% text grows card height. RTL mirrors grid while time/chart semantics follow locale. At 200% text, status/exit never clips; Automation exposes role, state, position, and next action.

## Theme and High Contrast

Use ThemeResource only. High Contrast does not depend on Acrylic/Mica/shadow/transparency; Reduced Motion removes slide/scale but preserves state.

## Copyright and Asset Exclusion Zone

Commit no Windows 11 screenshot, system icon, wallpaper, sound, font, news content, brand text, or extracted package asset; use owned geometry/fake data.

