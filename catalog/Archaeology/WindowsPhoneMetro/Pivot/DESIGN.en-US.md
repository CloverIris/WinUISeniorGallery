# Pivot Modern Design

## Prototype Visuals and Preservation

Visual structure is HeaderStrip, SelectedHeader, peeking neighbor headers, and one ContentViewport; it does not require every page realized. Preserve: Lightweight navigation, discoverable neighbors, content-sized headers, and one motion progress shared by header and content.

## Discarded Behavior

Discard gesture-only navigation, default infinite loop, clipped giant headers, and eager pages; add explicit click, focus, overflow, and virtualization.

## Input and Focus

Keep horizontal swipe; arrows/gamepad bumpers switch, Tab enters page content, screen reader announces item n of m. RTL mirrors visual order/direction. Focus uses modern system visuals and automatic motion never steals focus or changes Automation names.

## Responsive Behavior and Localization

Narrow keeps neighbor peek; wide caps HeaderStrip width and may show more headers without turning Pivot into a sidebar. Test Chinese, English, long text, and RTL with real resources without assuming Segoe/English metrics.

## Theme, Motion, and Accessibility

Modern Demo uses ThemeResource only. High Contrast does not depend on imagery/transparency; Reduced Motion removes parallax/scale/slide while preserving state. Targets are at least 44×44 epx.

## Asset Exclusion Zone

Commit no product screenshot, cover, artist image, trademark, sound, original font file, or extracted package resource; use owned geometry, license-compatible assets, and text diagrams only.

