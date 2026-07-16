# Zune Mixview Modern Design

## Prototype Visuals and Preservation

CenterNode, radial RelatedNodes, relationship-weighted sizes, connection semantics, and detail layer; not a generic physics graph. Preserve: Visible relationships, explicit center context, size encoding importance, and continued exploration through recentering.

## Discarded Behavior

Discard unexplained randomness, endless motion, color-only relationships, eager nodes, and keyboard-inaccessible graph; use deterministic layout, legend, paging, and list alternative.

## Input and Focus

Arrows/gamepad move spatially, Enter/A recenters, Escape/B returns prior center; provide linear relation list for Narrator/2D-inaccessible users. Focus uses modern system visuals and automatic motion never steals focus or changes Automation names.

## Responsive Behavior and Localization

Narrow reduces rings and uses bottom details; wide expands radius with node cap; text scaling grows nodes rather than compressing labels. Test Chinese, English, long text, and RTL with real resources without assuming Segoe/English metrics.

## Theme, Motion, and Accessibility

Modern Demo uses ThemeResource only. High Contrast does not depend on imagery/transparency; Reduced Motion removes parallax/scale/slide while preserving state. Targets are at least 44×44 epx.

## Asset Exclusion Zone

Commit no product screenshot, cover, artist image, trademark, sound, original font file, or extracted package resource; use owned geometry, license-compatible assets, and text diagrams only.

