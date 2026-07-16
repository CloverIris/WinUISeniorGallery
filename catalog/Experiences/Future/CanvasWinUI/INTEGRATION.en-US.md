# Canvas.WinUI Integration

## Dependencies

future.canvas-abstractions, future.canvas-native

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Host, privacy, and lifecycle
Host owns Window/DPI/Dispatcher/document/save; control coordinates input/surface and never persists/uploads ink. Unload stops and joins render thread.
