# Canvas.Abstractions Integration

## Dependencies

foundation.input-system

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Host, privacy, and lifecycle
Host owns format/storage/assets/recovery; abstraction accesses no file/clipboard/network. Native ABI passes immutable blocks with explicit ownership.
