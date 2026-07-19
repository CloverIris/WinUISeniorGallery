# MediaCenterGrid

Define a virtualized poster grid for TV, remote, and living-room distance, enlarging focus and showing details during 2D navigation.

## Status and Scope

- Status: in-progress / lab / P1
- Dependency: controls.content-rail
- The current code provides single-selection poster browsing, click/keyboard invocation, and focus-friendly navigation; virtualization and final visual acceptance remain.

## Host Boundary

Remain in the current page visual tree; create no window, full screen, or playback. Host supplies ItemsSource, templates, navigation, and details; external services own windows, playback, and loading.

## Documents and Agent Ownership

SPEC defines promotion gates, DESIGN visual/input, INTEGRATION lifecycle, and ACCEPTANCE Given/When/Then. Implementation is under `src/WinUI3.Senior.Media/MediaCenterGrid`.
