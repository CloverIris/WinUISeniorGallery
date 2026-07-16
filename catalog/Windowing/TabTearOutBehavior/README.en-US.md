# TabTearOutBehavior

Transactionally transfer tab data/view model from one window to a new window or another TabHost with rollback.

## Status and Scope

- Status: proposed / lab / P2
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Never reparent XAML across Windows; source/target hosts supply transferable descriptor, ContentFactory, and window factory. Behavior only coordinates transaction and focus.

## Agent Ownership

Only catalog/Windowing/TabTearOutBehavior; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

