# ConnectedTransitionBehavior

Coordinate same-Window connected transitions between source/target while preserving focus/navigation transaction and deterministic fade for cross-window/missing elements.

## Status and Scope

- Status: in-progress / lab / P2
- Dependency: contracts.motion
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

P0 candidate is same XamlRoot/Window only; move no real XAML and share no CompositionVisual across windows. Host supplies stable TransitionKey and navigation completion.

## Agent Ownership

Only catalog/Windowing/ConnectedTransitionBehavior; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

## Implementation notes

Connected transitions now have a sequence number, explicit running/completed/cancelled states,
progress reporting, interruption, reduced-motion completion, and an attached controller that
coordinates VisualStateManager without moving content across windows.
