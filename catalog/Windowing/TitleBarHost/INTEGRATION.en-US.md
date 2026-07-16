# TitleBarHost Integration

## Dependencies and Global Contracts

Depend on contracts.windowing and Windowing, Theme, Motion, Input, Accessibility, Localization, Resources contracts; never redefine shared resources or window identity.

## Platform APIs and Degradation

Use XAML SetTitleBar association and AppWindowTitleBar insets; without custom title bar, degrade to a page Header and register no non-client region.

## Lifecycle, Threading, and Cross-window Behavior

Own title-bar visuals and region descriptions only, not AppWindow creation/closure; host forwards regions to WindowChrome and owns system title/lifecycle. All Window/XAML work stays on owning DispatcherQueue and only immutable descriptors cross threads. After Window.Closed, cancel work, detach events, and ignore late callbacks.

## Errors, Capabilities, and Privacy

Platform failure is observable and returns to a stable state. Request no extra capability by default and log no window title, content, coordinate, or input.

## Resources

Candidate resources use TitleBarHost prefix; exact keys freeze before ready. Styles never hard-code DPI, caption inset, theme, or OS version.

