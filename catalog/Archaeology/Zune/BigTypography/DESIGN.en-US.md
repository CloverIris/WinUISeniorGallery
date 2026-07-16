# Zune Big Typography Modern Design

## Prototype Visuals and Preservation

BigTitle, optional Eyebrow/Subtitle, content baseline, and CompactTitle after scroll collapse/pin; typography itself defines hierarchy. Preserve: Bold scale, whitespace, light weight, content-first hierarchy, and continuous title transformation rather than sudden replacement.

## Discarded Behavior

Discard fixed 72pt, English lowercase assumptions, clipping, poor CJK/RTL adaptation, and low-contrast thin weight; use language-aware scale/weight and text scaling.

## Input and Focus

Title is noninteractive and out of Tab by default; if bound to navigation it gets button semantics/focus and no invisible hit area. Focus uses modern system visuals and automatic motion never steals focus or changes Automation names.

## Responsive Behavior and Localization

CJK uses suitable weight/scale, German wraps, RTL mirrors alignment; 200% text allows multiline/compact instead of clipping. Test Chinese, English, long text, and RTL with real resources without assuming Segoe/English metrics.

## Theme, Motion, and Accessibility

Modern Demo uses ThemeResource only. High Contrast does not depend on imagery/transparency; Reduced Motion removes parallax/scale/slide while preserving state. Targets are at least 44×44 epx.

## Asset Exclusion Zone

Commit no product screenshot, cover, artist image, trademark, sound, original font file, or extracted package resource; use owned geometry, license-compatible assets, and text diagrams only.

