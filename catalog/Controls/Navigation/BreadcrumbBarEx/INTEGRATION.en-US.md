# BreadcrumbBarEx Integration

## Dependencies

foundation.navigation-catalog

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
UI-thread parse/collection; suggestions are async/cancellable and unload cancels queries/drag. No file capability; host supplies parser.
