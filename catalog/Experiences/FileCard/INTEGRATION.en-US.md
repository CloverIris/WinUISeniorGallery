# FileCard Integration

## Dependencies

controls.widget-card

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes WidgetCard; host provider owns file/cloud capability, thumbnail, delete confirmation. Experience holds opaque Id and never opens/uploads.
