# Hub Modern Design

## Prototype Visuals and Preservation

HubHeader, horizontal SectionStrip, variable-width Sections, SectionHeader, background layer, and semantic jump entry form a continuous surface. Preserve: Heterogeneous content, horizontal exploration, section headings, global background continuity, and editorial emphasis on the first viewport.

## Discarded Behavior

Discard eager infinite width, uncontrolled background contrast, nested horizontal conflicts, and gesture-only entry; use section virtualization, scrims, and explicit jump.

## Input and Focus

Touch/touchpad pans, Shift+wheel and arrows/gamepad move by Section; nested lists own their main axis and B/Escape returns to host. Focus uses modern system visuals and automatic motion never steals focus or changes Automation names.

## Responsive Behavior and Localization

Narrow shows classic one-Section viewport; wide shows 1.5–2.5 Sections with capped body width; high DPI does not alter semantic span. Test Chinese, English, long text, and RTL with real resources without assuming Segoe/English metrics.

## Theme, Motion, and Accessibility

Modern Demo uses ThemeResource only. High Contrast does not depend on imagery/transparency; Reduced Motion removes parallax/scale/slide while preserving state. Targets are at least 44×44 epx.

## Asset Exclusion Zone

Commit no product screenshot, cover, artist image, trademark, sound, original font file, or extracted package resource; use owned geometry, license-compatible assets, and text diagrams only.

