# Guide Menu Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Side quick navigation descended from the Xbox 360 Guide button through Xbox One/Series; it overlays the current game with layered entry points for home, social, achievements, capture, and system actions.

## Design DNA

Always reachable, contextual overlay, stable top-level destinations, predictable hierarchical back, controller-first vertical focus, and bumper shortcuts.

## Modern Reconstruction

Reconstruct as an in-app Guide-style rail for immersive media or 10-foot apps. It carries app commands only and never replaces system navigation or intercepts reserved buttons.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by experiences.guide-menu (GuideMenuExperience). The exhibit declares no public types, resource keys, or platform services and never duplicates stable implementation. Gallery composes the modern dependency and labels the result Inspired by, never Original.

## States and Failure Modes

Closed, Root, Nested, and ModalChild; B/Escape backs out of children and closes at root, preserving and restoring prior focus. Missing dependency, data, or platform capability shows explanatory degradation and never fakes success. Closing the exhibit releases demo state and restores entry focus.
