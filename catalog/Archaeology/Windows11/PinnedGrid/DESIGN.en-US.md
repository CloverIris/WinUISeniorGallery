# Start Menu Pinned Grid Modern Design

## Prototype Structure and Design DNA

StartSurface contains PinnedHeader, fixed-column Grid, PageIndicator, FolderTile/FolderFlyout, AllApps entry, and Recommended section. Preserve: Pinned frequent entry, uniform icon rhythm, user reorder, folders for density, and paging that preserves predictable positions.

## Explicitly Discarded

Discard emulating Start, launching system apps, reading real pins, Recommended/ads, system folder drops, and Windows icon assets; extract an adaptive pinned grid only.

## Input and Focus

Arrows/gamepad navigate grid, Enter/A invokes, menu manages, and keyboard alternatives move/create folder; icons expose text names rather than graphics only. Automatic updates never steal focus or rename Automation; closing an overlay restores an explicit trigger.

## Responsive, RTL, and Automation

Generate 3–8 columns by minimum cell; narrow reduces columns/pages and wide caps total width. RTL mirrors columns/pages and stable ID retains focus. At 200% text, status/exit never clips; Automation exposes role, state, position, and next action.

## Theme and High Contrast

Use ThemeResource only. High Contrast does not depend on Acrylic/Mica/shadow/transparency; Reduced Motion removes slide/scale but preserves state.

## Copyright and Asset Exclusion Zone

Commit no Windows 11 screenshot, system icon, wallpaper, sound, font, news content, brand text, or extracted package asset; use owned geometry/fake data.

