# Captions.Abstractions Integration

## Dependencies

contracts.timed-text

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Host, privacy, and lifecycle
Host chooses audio, consent, provider, retention/deletion, redacted logging; abstraction holds no audio/text history. Cancellation/backpressure propagate end-to-end.
