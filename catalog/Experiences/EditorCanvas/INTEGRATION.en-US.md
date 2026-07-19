# EditorCanvas Integration

## Dependencies

`WinUI3.Senior.Core` CanvasDocumentController/CanvasViewportController and `WinUI3.Senior.Controls` EditorCanvas. Gallery supplies synthetic objects only.

## Global contracts and resources

Do not redefine global contracts or shared resource keys. There is no async work now; hosts detach Document and event subscriptions on unload. No capability, telemetry, or user-content collection is introduced.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

The document controller may receive cross-thread mutations, but Document binding and rendering return to the UI thread. Hosts detach Changed on destruction; late mutations never access a window.

## Ownership, boundaries, and lifecycle
The host owns document format, saving, and font/image licensing. Core normalization rejects invalid geometry and duplicate IDs. EditorCanvas creates no window, stores no file path, and owns no file handle.
