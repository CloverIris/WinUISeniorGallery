# TabbedShell Integration

## Dependencies

windowing.tab-tear-out-behavior

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes TabTearOutBehavior; Windowing creates windows, host creates/saves content; no arbitrary view serialization. Failure returns original.
