# TitleBarHost

Provide composable XAML title-bar layout that generates drag/interactive regions and synchronizes with host WindowChrome.

## Status and Scope

- Status: proposed / lab / P1
- Dependency: contracts.windowing
- Not eligible for implementation; candidate API/part names are review vocabulary only.

## Host and Window Boundary

Own title-bar visuals and region descriptions only, not AppWindow creation/closure; host forwards regions to WindowChrome and owns system title/lifecycle.

## Agent Ownership

Only catalog/Windowing/TitleBarHost; SPEC/DESIGN/INTEGRATION/ACCEPTANCE lock responsibility, visuals, platform lifecycle, and acceptance.

