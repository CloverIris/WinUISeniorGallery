# WindowChrome Integration

## Dependencies and Global Contracts

Depend on contracts.windowing and Windowing, Theme, Motion, Input, Accessibility, Localization, Resources contracts; never redefine shared resources or window identity.

## Platform APIs and Degradation

Use AppWindow, AppWindowTitleBar, SetTitleBar, and SystemBackdrop; unsupported backdrop falls to ThemeResource solid color without bypassing caption buttons.

## Lifecycle, Threading, and Cross-window Behavior

One instance binds one host Window/AppWindow; it creates no window, owns no page content, and changes no navigation. Host retains it, closes it, and presents errors. All Window/XAML work stays on owning DispatcherQueue and only immutable descriptors cross threads. After Window.Closed, cancel work, detach events, and ignore late callbacks.

## Errors, Capabilities, and Privacy

Platform failure is observable and returns to a stable state. Request no extra capability by default and log no window title, content, coordinate, or input.

## Resources

Candidate resources use WindowChrome prefix; exact keys freeze before ready. Styles never hard-code DPI, caption inset, theme, or OS version.

