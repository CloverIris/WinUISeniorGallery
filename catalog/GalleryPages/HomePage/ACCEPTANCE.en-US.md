# HomePage Acceptance

## Routing and loading

- Given `route.gallery.home` is registered, when navigating to `/`, then one page instance is created in the target window scope.
- Given valid `section, feature`, when loading, then normalized parameters, selection, and anchor are restorable.
- Given unknown parameters, when parsed, then defaults remain usable and one diagnostic is recorded.
- Given a warm snapshot, when navigation begins, then the page meets its interactive-frame budget.
- Given navigation away during load, when data completes, then the stale result is discarded.
- Given window close, when async work completes, then no update reaches another window.

## Content and states

- Given valid data, when Loaded applies, then every item has a unique ID and registered route.
- Given no matching data, when Empty appears, then retain the product statement and fixed category entries when the catalog is empty.
- Given provider failure, when Error appears, then Retry and a correlation ID are visible.
- Given permission/capability evaluation, when rendered, then request no permission or network.
- Given missing localization, when rendered, then en-US or stable ID replaces blank text.
- Given missing preview, when rendered, then a themed placeholder appears without network access.
- Given invalid target route, when rendered, then activation is disabled but metadata remains readable.

## Interaction and focus

- Given a focused primary action, when Enter/Space/gamepad A is pressed, then it will open the featured entry's canonical route exactly once.
- Given pointer double activation, when navigation is pending, then requests are coalesced.
- Given a transient surface, when Escape is pressed, then it closes and restores invoker focus.
- Given focused content is removed, when refresh applies, then focus moves to nearest surviving content.
- Given keyboard-only use, when traversing, then every primary and recovery action is reachable.
- Given Shift+F10, when a secondary menu exists, then it matches pointer secondary actions.

## Responsive, theme, and RTL

- Given 320 epx, when rendered, then one column shows all required text/actions without horizontal clipping.
- Given 640–1023 epx, when rendered, then columns respect minimum card width.
- Given 1024+ epx, when rendered, then content width is bounded and reading order stays logical.
- Given 200% text or 400% display scale, when rendered, then status/legal text and actions remain available.
- Given live Light/Dark change, when applied, then selection and page instance persist.
- Given High Contrast, when rendered, then material/shadows are unnecessary and boundaries remain visible.
- Given RTL, when rendered, then layout mirrors while IDs/code/versions preserve direction.
- Given Reduced Motion, when state changes, then translation/parallax/shimmer are absent.

## Automation and accessibility

- Given a screen reader, when navigation completes, then H1 is announced once.
- Given Loaded result changes, when announced, then one polite coalesced message is emitted.
- Given Error, when announced, then concise error is spoken once and correlation ID is selectable.
- Given a card, when queried, then Name, Description, Status, PositionInSet, and Invoke exist.
- Given skeletons, when queried, then they are absent from Automation.
- Given color-vision deficiency, when viewing status, then text and shape preserve meaning.

## Performance, privacy, and cleanup

- Given target catalog scale, when scrolling/filtering, then virtualization and latest-wins updates remain responsive.
- Given UI work over 50 ms, when diagnostics is enabled, then a content-free performance event is emitted.
- Given observability enabled, when events emit, then no personal/user content is included.
- Given unload, when cleanup completes, then tokens, events, timers, surfaces, and heavy assets are released.
- Given window close, when Snackbar/navigation completes, then it is cancelled rather than redirected.
- Given offline operation, when page loads, then packaged data remains complete without retry loops.

## Delivery gate

- UI Automation covers route load, activation, empty, error, and recovery.
- Visual tests cover Narrow/Medium/Wide, Light/Dark/High Contrast, RTL, and text scale.
- Performance tests report median and P95 using deterministic local data.
- No test requires network, sign-in, or copyrighted Microsoft UI assets.
- x64 passes; x86/ARM64 compatibility is exercised where infrastructure permits.
- Status remains proposed until implementation, automated evidence, accessibility review, and design review pass.
