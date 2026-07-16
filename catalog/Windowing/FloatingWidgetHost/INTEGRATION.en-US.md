# FloatingWidgetHost Integration

## Dependencies and Global Contracts

Depend on contracts.windowing and Windowing, Theme, Motion, Input, Accessibility, Localization, Resources contracts; never redefine shared resources or window identity.

## Platform APIs and Degradation

Use AppWindow/Window, OverlappedPresenter, and DispatcherQueue; multi-thread windows only when platform/app model permits, otherwise same-UI-thread multiwindow.

## Lifecycle, Threading, and Cross-window Behavior

Host owns secondary AppWindow and window services, while host ContentFactory creates content on target DispatcherQueue; never reparent main-window XAML instances. All Window/XAML work stays on owning DispatcherQueue and only immutable descriptors cross threads. After Window.Closed, cancel work, detach events, and ignore late callbacks.

## Errors, Capabilities, and Privacy

Platform failure is observable and returns to a stable state. Request no extra capability by default and log no window title, content, coordinate, or input.

## Resources

Candidate resources use FloatingWidgetHost prefix; exact keys freeze before ready. Styles never hard-code DPI, caption inset, theme, or OS version.

