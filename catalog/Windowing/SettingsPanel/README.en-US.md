# SettingsPanel

Provide an in-window settings side panel with hierarchical navigation, back, and unsaved-change policy.

## Status and Scope

- Status: in-progress / lab / P2
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Create no settings window, persist no settings, and decide no identity/permission; host supplies page factory, navigation stack, commit/cancel policy.

## Agent Ownership

Only catalog/Windowing/SettingsPanel; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.
