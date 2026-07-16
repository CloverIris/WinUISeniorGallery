# Zune Album Art Wall Modern Design

## Prototype Visuals and Preservation

Regular/semi-regular CoverGrid, cropped covers, selection emphasis, and optional dim background; text stays in a separate foreground. Preserve: Content becomes brand atmosphere, repeated grid creates rhythm, and current item stands out through scale/luminance rather than heavy borders.

## Discarded Behavior

Discard eager full-library load, unstable random layout, low-contrast text, and unlicensed imagery; use virtualization, stable IDs, scrim, placeholders, licensed assets.

## Input and Focus

Arrows/gamepad follow geometric neighbors and mouse/touch invoke; scale does not change focus geometry and screen reader announces album/artist, not image. Focus uses modern system visuals and automatic motion never steals focus or changes Automation names.

## Responsive Behavior and Localization

Narrow reduces columns and wide caps cover size; background mode lowers detail/update rate while browse mode keeps focus/text. Test Chinese, English, long text, and RTL with real resources without assuming Segoe/English metrics.

## Theme, Motion, and Accessibility

Modern Demo uses ThemeResource only. High Contrast does not depend on imagery/transparency; Reduced Motion removes parallax/scale/slide while preserving state. Targets are at least 44×44 epx.

## Asset Exclusion Zone

Commit no product screenshot, cover, artist image, trademark, sound, original font file, or extracted package resource; use owned geometry, license-compatible assets, and text diagrams only.

