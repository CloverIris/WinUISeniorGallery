# ASR Provider Integration

## Dependencies

future.captions-abstractions

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Host, privacy, and lifecycle
Host owns mic capability, keys, region, cost, consent UI, retention. Provider logs no audio/transcript and never callbacks after cancel.
