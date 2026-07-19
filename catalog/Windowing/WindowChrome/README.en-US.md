# WindowChrome

Coordinate custom title bar, system caption regions, backdrop, and non-client hit testing for one Window.

## Status and Scope

- Status: in-progress / lab / P1
- Dependency: contracts.windowing
- WindowChrome remains a candidate implementation; `WindowPlacementCoordinator` is now available as a host-neutral geometry state machine for Snap/custom-title-bar experiments.

`WindowPlacementCoordinator` only tracks the work area, bounds, min/max size, Normal/Maximized/FullScreen modes, move, eight-edge resize, and eight Snap targets. It never calls AppWindow/HWND; the host must apply its `Snapshot`.

## Host and Window Boundary

One instance binds one host Window/AppWindow; it creates no window, owns no page content, and changes no navigation. Host retains it, closes it, and presents errors.

## Agent Ownership

Only catalog/Windowing/WindowChrome; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.
