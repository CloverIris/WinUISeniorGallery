# CarouselView Design

## Information hierarchy and layout

`PART_Root` hosts a clipped primary-content region, optional previous/next buttons, optional indicators, and an invisible Live Region. The selected item is the visual primary layer; adjacent realized items are visible only when `IsAdjacentPreviewEnabled=true`. Narrow windows prioritize the primary touch target, while regular and wide windows may show adjacent previews that never obscure titles, focus visuals, or buttons.

## Visuals and theme

The control must use only the `CarouselViewBackground`, `CarouselViewNavigationButtonStyle`, `CarouselViewIndicatorForeground`, `CarouselViewIndicatorSelectedForeground`, `CarouselViewItemSpacing`, `CarouselViewTransitionDuration`, and `CarouselViewCornerRadius` theme resources. Light, Dark, High Contrast, and runtime theme changes must not recreate the data source. In high contrast, system foreground/background and visible focus take precedence; opacity must not be the only state signal.

## Interaction and motion

Buttons, keyboard, wheel, drag, and controller input ultimately commit the same selection change. Drag enters `Dragging`, then reaches `Settling` or `Idle`; cancellation, unload, and collection changes must end a gesture safely. `Slide`, `Fade`, and `CoverFlow` are Composition enhancements and never change selection semantics. Reduced Motion or unavailable Composition limits transitions to 100ms or converges immediately.

## Input, focus, and accessibility

Left/right direction mirrors in LTR/RTL while Home/End retain logical first/last semantics. Ordinary vertical wheel is not captured. Focus order is Previous (when present), content, Next (when present), indicator (when present); focus entry/exit pauses or resumes autoplay according to configuration. Automation provides Selection, Scroll, and ItemContainer; Live Region announces only user actions. When a touchpad cannot be reliably identified, the device kind is `Unknown`.

## Responsive behaviour and modernization trade-offs

The control must remain fully operable at 320, 800, and 1920 effective-pixel widths and 100%, 150%, and 200% DPI, with no clipped target during text scaling. Chinese, English, and RTL must not assume fixed text width or LTR geometry. The design draws from horizontal media browsing while copying no historical visual asset.
