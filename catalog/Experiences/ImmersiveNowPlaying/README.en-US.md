# ImmersiveNowPlaying

Specification work item for ImmersiveNowPlaying. The reusable coordination layer currently lives in the Media package.

## Status

in-progress / lab / P1. Implementation is active; visual/template work remains.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Experiences/ImmersiveNowPlaying  
src/WinUI3.Senior.Media/NowPlaying

## Scenario readiness
Immersive current media, lyrics, and controls. The current code provides host-owned session binding, queue selection, serialized playback commands, repeat/shuffle intent, and fullscreen requests; it never creates windows, opens files, or parses captions. Ready after no-art/long-lyrics/video prototypes lock hierarchy and template/visual acceptance is complete.
