# WidgetCard Integration

## Dependencies

foundation.theme-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Hosts inject commands; the control starts no timer/network. Unload detaches visual subscriptions. Properties are UI-thread only; refresh failure enters Error while retaining prior content.
