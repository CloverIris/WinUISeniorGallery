# Semantic Zoom Research Exhibit Acceptance

- Given the exhibit opens, when overview is selected, then show groups, historical state, and `controls.semantic-zoom-view` link without calling a control API.
- Given a group is active, when returning to detail, then explain the anchor; without mapping show `NoGroupMapping`, never silently jump.
- Given keyboard, narrow window, RTL, 200% DPI, High Contrast, or Reduced Motion, when changing level, then the task completes with visible focus, unclipped text, and non-required animation.

Automation asserts level names, group labels, Esc/Back return, owner link, no network/telemetry, and local first frame; cover Chinese/English/RTL at 100/150/200% DPI. Only reviewed self-made assets may ship.
