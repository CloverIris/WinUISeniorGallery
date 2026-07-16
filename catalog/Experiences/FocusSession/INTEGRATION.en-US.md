# FocusSession Integration

## Dependencies

controls.snackbar-host

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes SnackbarHost; host supplies tasks/optional DND provider. No system-setting capability/background guarantee; resume uses elapsed-time policy.
