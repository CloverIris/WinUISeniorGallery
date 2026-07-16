# RevealBorderBehavior

Provide pointer-proximity border illumination for in-app interactive elements with safe modern-theme/accessibility degradation.

## Status and Scope

- Status: proposed / lab / P2
- Dependency: contracts.motion
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Attach only to XAML elements in the current Window; create no light window, track no global pointer, and change no hit testing/command.

## Agent Ownership

Only catalog/Windowing/RevealBorderBehavior; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

