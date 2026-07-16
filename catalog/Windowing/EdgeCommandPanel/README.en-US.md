# EdgeCommandPanel

Present a draggable command panel at an app-window edge, preserving progressive disclosure without imitating system edge UI.

## Status and Scope

- Status: proposed / lab / P2
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Work only inside host XAML root; register no global edge gesture, cover no other app, and call no system Charms. Host supplies commands and opening policy.

## Agent Ownership

Only catalog/Windowing/EdgeCommandPanel; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

