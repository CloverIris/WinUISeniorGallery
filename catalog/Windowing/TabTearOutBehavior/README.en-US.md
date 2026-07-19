# TabTearOutBehavior

Transactionally transfer tab data/view model from one window to a new window or another TabHost with rollback.

## Status and Scope

- Status: in-progress / lab / P2
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Never reparent XAML across Windows; source/target hosts supply transferable descriptor, ContentFactory, and window factory. Behavior only coordinates transaction and focus.

## Agent Ownership

Only catalog/Windowing/TabTearOutBehavior; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

## Implementation notes

The controller now tracks pressed/dragging/requested/cancelled states, uses a two-dimensional
distance threshold, reports drag progress, supports cancellation and an explicit host-triggered
tear-out request, and still leaves window creation and content transfer to the host.
