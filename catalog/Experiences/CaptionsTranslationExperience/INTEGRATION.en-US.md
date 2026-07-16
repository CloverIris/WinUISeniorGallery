# CaptionsTranslationExperience Integration

## Dependencies

media.timed-text-view, future.captions-abstractions

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes TimedTextView and Future Captions; host chooses provider/credentials/consent/retention. Experience records/uploads/logs nothing and propagates cancellation.
