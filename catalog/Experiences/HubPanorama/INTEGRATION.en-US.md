# HubPanorama Integration

## Dependencies

controls.pivot-view

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes PivotView but owns Section orchestration; host provides data/images; unload cancels prefetch/Composition. No system Panorama API.
