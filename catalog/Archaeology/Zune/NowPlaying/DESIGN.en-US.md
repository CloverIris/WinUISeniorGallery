# Zune Now Playing Modern Design

## Prototype Visuals and Preservation

AmbientBackground, Artist/AlbumVisual, Metadata, Progress/Transport overlay, and optional queue/lyrics; content and control layers are separate. Preserve: Immersive content-first hierarchy, media-derived color, progressively disclosed controls, and title/image storytelling.

## Discarded Behavior

Discard default network artist downloads, low-contrast text on art, focus loss from auto-hide, and continuous background motion; use licensed Provider, scrim, focus retention, Reduced Motion.

## Input and Focus

Any input reveals controls; focus/menu/scrub prevents hiding. Space/A plays, arrows use timeline, B/Escape exits immersive; background is not hit-testable. Focus uses modern system visuals and automatic motion never steals focus or changes Automation names.

## Responsive Behavior and Localization

Small window becomes standard player and wide retains atmosphere; CompactOverlay does not copy full-screen background. High Contrast uses solid color, not art extraction. Test Chinese, English, long text, and RTL with real resources without assuming Segoe/English metrics.

## Theme, Motion, and Accessibility

Modern Demo uses ThemeResource only. High Contrast does not depend on imagery/transparency; Reduced Motion removes parallax/scale/slide while preserving state. Targets are at least 44×44 epx.

## Asset Exclusion Zone

Commit no product screenshot, cover, artist image, trademark, sound, original font file, or extracted package resource; use owned geometry, license-compatible assets, and text diagrams only.

