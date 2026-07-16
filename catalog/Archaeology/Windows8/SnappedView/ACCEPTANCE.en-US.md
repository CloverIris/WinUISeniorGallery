# Snapped View Research Exhibit Acceptance

- Given the exhibit opens, when a historical layout is selected, then show primary region, snap region, state explanation, and `windowing.dock-layout-preview` link with no window operation.
- Given simulated preview, when Esc, Back, or Cancel is used, then return to `Single` and restore focus to the trigger.
- Given narrow window/200% DPI/RTL/High Contrast/Reduced Motion, when layout changes, then no clipping occurs, direction is correct, focus is visible, and the task completes without animation.
- Given no host/capability, when “apply example” is selected, then clearly fall back without persisting layout or calling platform API.

Automation checks route, all state text, keyboard order, accessible names, and no window calls; capture Chinese/English/RTL at three DPI levels. Only reviewed self-made assets may ship.
