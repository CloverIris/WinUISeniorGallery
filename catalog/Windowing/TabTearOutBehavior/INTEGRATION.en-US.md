# TabTearOutBehavior Integration

## Dependencies and Global Contracts

Depend on contracts.windowing and Windowing, Theme, Motion, Input, Accessibility, Localization, Resources contracts; never redefine shared resources or window identity.

## Platform APIs and Degradation

Use AppWindow/Window factory, DispatcherQueue, and in-app drag; cross-thread transfer carries immutable descriptor/model handle, never DependencyObject.

## Lifecycle, Threading, and Cross-window Behavior

Never reparent XAML across Windows; source/target hosts supply transferable descriptor, ContentFactory, and window factory. Behavior only coordinates transaction and focus. All Window/XAML work stays on owning DispatcherQueue and only immutable descriptors cross threads. After Window.Closed, cancel work, detach events, and ignore late callbacks.

## Errors, Capabilities, and Privacy

Platform failure is observable and returns to a stable state. Request no extra capability by default and log no window title, content, coordinate, or input.

## Resources

Candidate resources use TabTearOutBehavior prefix; exact keys freeze before ready. Styles never hard-code DPI, caption inset, theme, or OS version.

