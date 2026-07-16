# RevealBorderBehavior Integration

## Dependencies and Global Contracts

Depend on contracts.motion and Windowing, Theme, Motion, Input, Accessibility, Localization, Resources contracts; never redefine shared resources or window identity.

## Platform APIs and Degradation

Use Microsoft.UI.Composition/XamlCompositionBrushBase or fallback; unsupported effects, remote desktop, or power policy uses static ThemeResource border.

## Lifecycle, Threading, and Cross-window Behavior

Attach only to XAML elements in the current Window; create no light window, track no global pointer, and change no hit testing/command. All Window/XAML work stays on owning DispatcherQueue and only immutable descriptors cross threads. After Window.Closed, cancel work, detach events, and ignore late callbacks.

## Errors, Capabilities, and Privacy

Platform failure is observable and returns to a stable state. Request no extra capability by default and log no window title, content, coordinate, or input.

## Resources

Candidate resources use RevealBorderBehavior prefix; exact keys freeze before ready. Styles never hard-code DPI, caption inset, theme, or OS version.

