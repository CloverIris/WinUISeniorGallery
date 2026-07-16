# FontPicker Integration

## Dependencies

foundation.localization-service

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Enumeration is async/cancellable; provider handles system/app fonts without default file access. Unload cancels/releases FontFamily cache.
