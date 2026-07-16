# Application Bar Modern Design

## Prototype Visuals and Preservation

PrimaryIconRow, EllipsisButton, PrimaryLabels, SecondaryMenu, and translucent Backplate; primary icons remain when collapsed. Preserve: Thumb-reachable bottom actions, four-primary limit, progressive disclosure, text labels on expand, and retained content context.

## Discarded Behavior

Discard ellipsis guesswork, fixed phone height, icons without ToolTips, and keyboard-inaccessible secondary commands; add explicit names, overflow, and focus.

## Input and Focus

Touch upward drag or ellipsis expands; keyboard/gamepad reaches every command; Escape/B closes by layer and restores trigger/content focus. Focus uses modern system visuals and automatic motion never steals focus or changes Automation names.

## Responsive Behavior and Localization

Narrow/touch favors bottom; wide may stay bottom with capped width, not automatically top. Host supplies safe-area and input-pane insets. Test Chinese, English, long text, and RTL with real resources without assuming Segoe/English metrics.

## Theme, Motion, and Accessibility

Modern Demo uses ThemeResource only. High Contrast does not depend on imagery/transparency; Reduced Motion removes parallax/scale/slide while preserving state. Targets are at least 44×44 epx.

## Asset Exclusion Zone

Commit no product screenshot, cover, artist image, trademark, sound, original font file, or extracted package resource; use owned geometry, license-compatible assets, and text diagrams only.

