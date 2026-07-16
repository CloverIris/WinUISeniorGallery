# Media Center Grid Modern Design

## Prototype Visuals and Preservation

GridViewport, virtualized poster rows/columns, FocusChrome, Page/SectionHeader, and DetailsOverlay; selection and focus may differ. Preserve: Stable 2D space, 10-foot focus, poster-first content, focus-driven details, and clear Back path.

## Discarded Behavior

Discard fixed columns, low-resolution bitmaps, remote-only input, scale relayout, and eager realization; use responsive columns, licensed images, render transform, and virtualization.

## Input and Focus

D-pad/stick follows geometric neighbors, A/Enter invokes, B/Escape returns; mouse/touch/keyboard equal. Automation exposes row/column, title, selection, and details. Automatic motion never steals focus or changes Automation names.

## Responsive Behavior and Localization

Reflow 1–8 columns by minimum card width and retain stable-ID focus; text scaling reduces columns, not font; High Contrast uses system focus ring. Test Chinese, English, long text, and RTL with real resources.

## Theme, Motion, and Accessibility

Demo uses ThemeResource only; High Contrast does not depend on image/transparency, Reduced Motion removes parallax/scale/slide but preserves state; targets are at least 44×44 epx.

## Asset Exclusion Zone

Forbid product screenshots, posters/covers, trademarks, sounds, original fonts, and extracted package resources; use owned geometry, license-compatible assets, and text diagrams.

