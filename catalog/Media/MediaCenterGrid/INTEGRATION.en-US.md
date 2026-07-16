# MediaCenterGrid Integration

## Dependencies and Global Contracts

Depend on controls.content-rail and applicable Theme, Motion, Input, Accessibility, Localization, Resources, MediaPlayback/Windowing contracts; never redefine shared keys.

## Host and Platform APIs

ItemsRepeater, ScrollViewer, and Composition with no window/network/media capability. Image Provider decodes off-thread and submits versioned UI results.

## Lifecycle, Threading, and Cross-window Behavior

Remain in the current page visual tree; create no window, full screen, or playback. Host supplies ItemsSource, templates, navigation, and details; external services own windows, playback, and loading. Visual work stays on owning DispatcherQueue; background results carry version/cancellation. Ignore late callbacks after destruction, Window.Closed, or unload.

## Degradation and Errors

Missing capability/part/data preserves an accessible minimum path and localized reason; never report false success or log media/window/input data.

## Resources

Candidate resources use MediaCenterGrid prefix; exact keys freeze before ready. Never hard-code color, DPI, or Window size.

