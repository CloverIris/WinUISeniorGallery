# MediaPlayerChrome

## Positioning

`MediaPlayerChrome` is a modern playback control layer decoupled from the playback engine. It consumes `IMediaPlaybackSession` and presents transport, timeline, volume, playback-rate, full-screen, and compact-overlay requests consistently.

## Scope

- Includes control UI, state mapping, command forwarding, auto-hide, and error/buffering feedback.
- Excludes decoding, source loading, window creation, cross-window content migration, and system media transport registration.
- P0 provides `MediaPlayerElementPlaybackSessionAdapter`, while the control never depends on the concrete `MediaPlayerElement` type.

## Status

- Work item: `ready`
- Maturity: `lab`
- Priority: `P0`
- Package: `WinUI3.Senior.Media`

## Dependencies

- `foundation.media-playback`
- `media.media-timeline`

## Documents

- [Specification](SPEC.en-US.md)
- [Design](DESIGN.en-US.md)
- [Integration](INTEGRATION.en-US.md)
- [Acceptance](ACCEPTANCE.en-US.md)

## Agent Ownership

An implementation agent may change only `feature.json` `owned_paths`. If the shared playback contract is insufficient, mark this item `blocked`; do not duplicate the contract locally.
