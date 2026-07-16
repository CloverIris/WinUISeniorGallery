# WindowChrome

Coordinate custom title bar, system caption regions, backdrop, and non-client hit testing for one Window.

## Status and Scope

- Status: proposed / lab / P1
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

One instance binds one host Window/AppWindow; it creates no window, owns no page content, and changes no navigation. Host retains it, closes it, and presents errors.

## Agent Ownership

Only catalog/Windowing/WindowChrome; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

