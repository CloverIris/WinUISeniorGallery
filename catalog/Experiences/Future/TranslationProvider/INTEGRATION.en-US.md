# Translation Provider Integration

## Dependencies

future.captions-abstractions

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Host, privacy, and lifecycle
Host owns credentials, region, cost, glossary, consent, retention. Provider never caches/telemeters text; logs only IDs/timing/error codes.
