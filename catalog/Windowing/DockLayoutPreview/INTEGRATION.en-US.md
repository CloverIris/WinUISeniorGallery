# DockLayoutPreview Integration

## Dependencies and Global Contracts

Depend on contracts.windowing and Windowing, Theme, Motion, Input, Accessibility, Localization, Resources contracts; never redefine shared resources or window identity.

## Platform APIs and Degradation

XAML drag/pointer and coordinate transforms with no OS window-management capability; leaving client area cancels or hands off to a dedicated Windowing host.

## Lifecycle, Threading, and Cross-window Behavior

Layout app-internal panels only; move no OS windows, call no system Snap, and cover no caption hover menu. Host owns dragged object and final layout transaction. All Window/XAML work stays on owning DispatcherQueue and only immutable descriptors cross threads. After Window.Closed, cancel work, detach events, and ignore late callbacks.

## Errors, Capabilities, and Privacy

Platform failure is observable and returns to a stable state. Request no extra capability by default and log no window title, content, coordinate, or input.

## Resources

Candidate resources use DockLayoutPreview prefix; exact keys freeze before ready. Styles never hard-code DPI, caption inset, theme, or OS version.

