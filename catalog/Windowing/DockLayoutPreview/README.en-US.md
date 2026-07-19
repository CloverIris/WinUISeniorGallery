# DockLayoutPreview

Show in-app dock targets and magnetic preview during content drag without copying Windows system Snap Layouts.

## Status and Scope

- Status: in-progress / lab / P2
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Layout app-internal panels only; move no OS windows, call no system Snap, and cover no caption hover menu. Host owns dragged object and final layout transaction.

## Agent Ownership

Only catalog/Windowing/DockLayoutPreview; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

## Implementation notes

The preview now exposes stable layout IDs, normalized zones, deterministic keyboard focus
movement, point-to-zone selection, DIP coordinate conversion, and a commit request carrying the
chosen layout and zone. Applying a layout remains the host application's responsibility.
