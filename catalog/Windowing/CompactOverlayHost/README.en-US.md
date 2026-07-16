# CompactOverlayHost

Reliably switch one AppWindow between Inline and CompactOverlay presenters while preserving/restoring size and host-confirmed state.

## Status and Scope

- Status: proposed / lab / P1
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Create no second window, migrate no XAML content, and control no playback; host supplies existing AppWindow, handles close, and authorizes requests.

## Agent Ownership

Only catalog/Windowing/CompactOverlayHost; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

