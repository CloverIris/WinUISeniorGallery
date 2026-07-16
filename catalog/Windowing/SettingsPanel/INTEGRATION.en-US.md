# SettingsPanel Integration

## Dependencies and Global Contracts

Depend on contracts.windowing and Windowing, Theme, Motion, Input, Accessibility, Localization, Resources contracts; never redefine shared resources or window identity.

## Platform APIs and Degradation

Pure XAML navigation/composition with no window capability; host Provider owns setting storage, permission, and cross-window synchronization.

## Lifecycle, Threading, and Cross-window Behavior

Create no settings window, persist no settings, and decide no identity/permission; host supplies page factory, navigation stack, commit/cancel policy. All Window/XAML work stays on owning DispatcherQueue and only immutable descriptors cross threads. After Window.Closed, cancel work, detach events, and ignore late callbacks.

## Errors, Capabilities, and Privacy

Platform failure is observable and returns to a stable state. Request no extra capability by default and log no window title, content, coordinate, or input.

## Resources

Candidate resources use SettingsPanel prefix; exact keys freeze before ready. Styles never hard-code DPI, caption inset, theme, or OS version.

