# EdgeCommandPanel Integration

## Dependencies and Global Contracts

Depend on contracts.windowing and Windowing, Theme, Motion, Input, Accessibility, Localization, Resources contracts; never redefine shared resources or window identity.

## Platform APIs and Degradation

Pure XAML/Composition and pointer capture with no window capability; lost touch/pen capture settles to nearest stable state.

## Lifecycle, Threading, and Cross-window Behavior

Work only inside host XAML root; register no global edge gesture, cover no other app, and call no system Charms. Host supplies commands and opening policy. All Window/XAML work stays on owning DispatcherQueue and only immutable descriptors cross threads. After Window.Closed, cancel work, detach events, and ignore late callbacks.

## Errors, Capabilities, and Privacy

Platform failure is observable and returns to a stable state. Request no extra capability by default and log no window title, content, coordinate, or input.

## Resources

Candidate resources use EdgeCommandPanel prefix; exact keys freeze before ready. Styles never hard-code DPI, caption inset, theme, or OS version.

