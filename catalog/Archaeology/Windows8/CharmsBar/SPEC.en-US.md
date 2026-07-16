# Charms Bar Research Specification

## Goals and non-goals

The goal is to document “contextual commands appear when needed while primary content stays in place”: a right vertical rail, five semantic entry points, and availability that changes with the current app/content. Non-goals are global hot corners, intercepting `Win+C`, system Share/Devices/Start access, or presenting historical names as public control APIs.

## Historical structure and states

The historical tree is `Screen → EdgeAffordance → CharmRail → CharmItem → ContextPane`. Exhibit-only states are `Closed`, `Hinted`, `RailVisible`, `ContextPaneVisible`, and `Unavailable`. Each item carries an icon, label, availability, and explanation; Settings could open an app-local second pane while Share/Devices depended on context.

## Input, failure, and fallback

Historical entry included right-edge swipe, hot corner, mouse, and `Win+C`; Gallery permits buttons, keyboard focus, and simulated swipe only. With no touch/animation, show a static rail. In narrow windows, without context, or when the host rejects a request, state that the historical capability cannot execute; never fake a system call. Any `PART_*`, AutomationId, or command belongs to the future `EdgeCommandPanel` specification, not this exhibit.
