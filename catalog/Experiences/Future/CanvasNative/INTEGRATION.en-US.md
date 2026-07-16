# Canvas.Native Integration

## Dependencies

future.canvas-abstractions

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Host, privacy, and lifecycle
Host owns GPU/thread/crash isolation/binary license. Native reads no file/network/clipboard and logs no ink. ABI failure safely falls back to CPU placeholder.
