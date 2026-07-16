# FloatingWidgetHost

Create and manage app-owned secondary floating-widget windows with topmost, minimize, and owner-window lifecycle policy.

## Status and Scope

- Status: proposed / lab / P1
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Host owns secondary AppWindow and window services, while host ContentFactory creates content on target DispatcherQueue; never reparent main-window XAML instances.

## Agent Ownership

Only catalog/Windowing/FloatingWidgetHost; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

