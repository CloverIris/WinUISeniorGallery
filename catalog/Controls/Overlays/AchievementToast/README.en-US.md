# AchievementToast

Specification work item for AchievementToast.

## Status

in-progress / lab / P2. Local FIFO, timed dismissal, and template binding are implemented.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Controls/Overlays/AchievementToast

## Implementation readiness
Each window owns an independent FIFO queue. `ShowAsync` accepts an immutable request and displays icon, title, description, progress, and rarity. Pending requests resolve as `HostDestroyed` when the host is disposed. It does not reuse Snackbar or create a window.
