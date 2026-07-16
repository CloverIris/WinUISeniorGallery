# MixviewExperience Integration

## Dependencies

foundation.motion-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Host supplies cancellable relation provider; no recommendation/persistence. Uses Motion contract; unload cancels layout worker/cache.
