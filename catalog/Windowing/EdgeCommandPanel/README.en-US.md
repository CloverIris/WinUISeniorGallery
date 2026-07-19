# EdgeCommandPanel

Present a draggable command panel at an app-window edge, preserving progressive disclosure without imitating system edge UI.

## Status and Scope

- Status: in-progress / lab / P2
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Work only inside host XAML root; register no global edge gesture, cover no other app, and call no system Charms. Host supplies commands and opening policy.

## Agent Ownership

Only catalog/Windowing/EdgeCommandPanel; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

## Implementation notes

The current implementation provides an edge-aware state machine (`Closed`, `Opening`, `Open`,
`Dragging`, `Closing`), host-neutral snap distances, keyboard/Home/End/Escape handling, light
dismissal, and a drag threshold. It only changes XAML state and raises requests; it never creates
a Popup, Window, or global edge gesture.
