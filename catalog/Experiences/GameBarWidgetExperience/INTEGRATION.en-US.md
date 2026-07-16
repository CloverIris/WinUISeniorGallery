# GameBarWidgetExperience Integration

## Dependencies

windowing.floating-widget-host

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes FloatingWidgetHost; Windowing owns styles, host owns content/capabilities. No game injection/Xbox API; destruction releases hotkey.
