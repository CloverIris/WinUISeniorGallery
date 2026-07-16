# ImmersiveReader Integration

## Dependencies

media.timed-text-view

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
May compose TimedTextView; host supplies document/cancellable TTS. No text upload/content persistence; host may save settings.
