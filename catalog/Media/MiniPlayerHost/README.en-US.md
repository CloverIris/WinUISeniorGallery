# MiniPlayerHost

Define an in-window host that collapses a full player into an edge mini player while preserving one playback session, focus return, and content state.

## Status and Scope

- Status: in-progress / lab / P1
- Dependency: media.media-player-chrome
- The current code provides an in-window compact host with play/pause, restore, and dismiss requests; final theme and acceptance work remains.

## Host Boundary

Manage two slots inside the current Window; create no AppWindow, request no CompactOverlay, and never reparent XAML across windows. CompactOverlayHost/DetachablePlayerHost owns windowing and MediaPlayerChrome/IMediaPlaybackSession owns playback.

## Documents and Agent Ownership

SPEC defines promotion gates, DESIGN visual/input, INTEGRATION lifecycle, and ACCEPTANCE Given/When/Then. Implementation is under `src/WinUI3.Senior.Media/MediaPlayerChrome/MiniPlayerHost.cs`.
