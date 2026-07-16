# AchievementToast Integration

## Dependencies

foundation.motion-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Threading and lifecycle
Explicit window target and DispatcherQueue serialization; unload/destruction completes all tasks, no system-toast capability.
