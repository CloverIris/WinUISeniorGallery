# WidgetsBoard Integration

## Dependencies

controls.widget-card, controls.adaptive-grid

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes WidgetCard/AdaptiveGrid; host registers factories/refresh/storage. Content sandbox/capabilities are host scope; unload cancels refresh.
