# Game Bar Widget Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Xbox Game Bar overlay widgets on Windows 10, introduced in the 2019-era redesign; Win+G reveals movable, resizable, pinnable, translucent panels above gameplay.

## Design DNA

An overlay that does not interrupt foreground content, independent widget window state, pin/unpin, compact and desktop modes, and continuous 2D focus navigation across gamepad and mouse.

## Modern Reconstruction

Reconstruct as an in-app floating-widget experience: the host supplies a workspace and widgets can move, resize, minimize, and stay on top. It never impersonates system Game Bar or injects into other processes.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by experiences.game-bar-widget (GameBarWidgetExperience). The exhibit declares no public types, resource keys, or platform services and never duplicates stable implementation. Gallery composes the modern dependency and labels the result Inspired by, never Original.

## States and Failure Modes

Windowed, pinned, unpinned, compact, unfocused, and input-device transitions preserve content state; host shutdown reclaims the surface safely. Missing dependency, data, or platform capability shows explanatory degradation and never fakes success. Closing the exhibit releases demo state and restores entry focus.
