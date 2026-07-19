# GuideMenuExperience Acceptance

## Current gate

`in-progress`; the local lab is available, while visual, Automation, and build verification remain before `review`.

## Given / When / Then

- Given a root node has children, When `Invoke(child)` is called, Then navigation depth increases and current items refresh.
- Given a child level is open, When Escape is pressed, Then the parent level is restored instead of closing.
- Given a leaf node, When `Invoke(leaf)` is called, Then `NodeInvoked` fires and closing follows `IsDismissOnLeafInvoke`.
- Given an empty collection or unknown ID, When a public method is called, Then it returns false and preserves state.

## Matrix

Light/Dark/High Contrast, RTL, 100%/150%/200% DPI, keyboard/mouse/touch, Narrator, and Reduced Motion.
