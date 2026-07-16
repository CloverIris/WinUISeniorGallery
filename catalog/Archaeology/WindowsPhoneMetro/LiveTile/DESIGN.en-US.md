# Live Tile Modern Design

## Prototype Visuals and Preservation

TileSurface contains brand, Primary/SecondaryContent, Badge, and size variant; the system schedules updates and Start owns layout. Preserve: Glanceable information, fixed size variants, brief dynamic rotation, unified identity/status, and user-owned layout.

## Discarded Behavior

Do not emulate OS Start, background Tile APIs, endless flips, or uncontrolled notification; modernize as an in-app dashboard card with pause.

## Input and Focus

Click/Enter/A invokes and outer Board owns keyboard/touch reorder; dynamic content does not rename focused tile and screen reader announces aggregate state, not every flip. Focus uses modern system visuals and automatic motion never steals focus or changes Automation names.

## Responsive Behavior and Localization

Sizes use modern grid breakpoints, not WP pixels; narrow has at least two columns and text scaling grows height or hides secondary copy. Test Chinese, English, long text, and RTL with real resources without assuming Segoe/English metrics.

## Theme, Motion, and Accessibility

Modern Demo uses ThemeResource only. High Contrast does not depend on imagery/transparency; Reduced Motion removes parallax/scale/slide while preserving state. Targets are at least 44×44 epx.

## Asset Exclusion Zone

Commit no product screenshot, cover, artist image, trademark, sound, original font file, or extracted package resource; use owned geometry, license-compatible assets, and text diagrams only.

