# ConnectedTransitionBehavior Integration

## Dependencies and Global Contracts

Depend on contracts.motion and Windowing, Theme, Motion, Input, Accessibility, Localization, Resources contracts; never redefine shared resources or window identity.

## Platform APIs and Degradation

Use Microsoft.UI.Composition and XamlRoot coordinates; across windows/DispatcherQueues or without effects, fade only and never share coordinates.

## Lifecycle, Threading, and Cross-window Behavior

P0 candidate is same XamlRoot/Window only; move no real XAML and share no CompositionVisual across windows. Host supplies stable TransitionKey and navigation completion. All Window/XAML work stays on owning DispatcherQueue and only immutable descriptors cross threads. After Window.Closed, cancel work, detach events, and ignore late callbacks.

## Errors, Capabilities, and Privacy

Platform failure is observable and returns to a stable state. Request no extra capability by default and log no window title, content, coordinate, or input.

## Resources

Candidate resources use ConnectedTransitionBehavior prefix; exact keys freeze before ready. Styles never hard-code DPI, caption inset, theme, or OS version.

