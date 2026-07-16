# EditorCanvas Integration

## Dependencies

future.canvas-winui

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Depends only on Future CanvasWinUI; host owns format/save/font/image licensing. No implementation before ABI; failure never corrupts source.
