# Charms Bar Research Exhibit Acceptance

## Scenarios

- Given Gallery opens the exhibit, when the user selects “show historical structure”, then show the rail, five semantics, and modern-owner link without executing a system command.
- Given the rail is visible, when Esc or Back is pressed, then close the context pane before the rail and restore focus to the trigger.
- Given a narrow window, High Contrast, RTL, 200% DPI, or Reduced Motion, when opened, then labels remain readable, focus is visible, logical end is correct, and no task depends on animation completion.
- Given no modern host or a rejected host request, when an example action is selected, then explain that it cannot execute without creating a window, registering shortcuts, or leaking to another window.

## Automation and gate

Verify route, title, owner link, per-layer Esc behavior, accessible names, and absence of system capabilities. Test 100/150/200% DPI, Light/Dark/High Contrast, Chinese/English/RTL with screenshots or UI automation. Initial local render must not await network; only license-reviewed self-made assets may ship.
