# WizardStepper Integration

## Dependencies

foundation.navigation-catalog

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Validation is cancellable and commits via UI Dispatcher; unload cancels. Host supplies persistence.
