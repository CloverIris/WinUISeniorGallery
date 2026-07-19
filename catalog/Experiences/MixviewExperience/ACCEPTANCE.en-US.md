# MixviewExperience Acceptance

## Current gate

The item is `in-progress`: the local Gallery lab is available, but visual, Automation, and build verification are required before `review`.

## Given / When / Then

- Given five deterministic nodes, When the root opens, Then it is centered and related nodes orbit in stable order.
- Given an open graph, When a user clicks a related node, Then `NodeSelected.IsUserInitiated=true`, state updates, and the live region announces the title.
- Given an open graph, When Escape is pressed, Then the graph closes and raises `Closed` without page navigation.
- Given an empty collection or unknown ID, When `Open`/`SelectNode` is called, Then it returns false and preserves state.

## Matrix and budget

Light/Dark/High Contrast, Reduced Motion, RTL, 100%/150%/200% DPI, keyboard/mouse/touch, and Narrator. The default renders at most 13 node buttons; redraw target is P95 ≤16ms (runtime evidence pending).
