# MediaPlayback

## Positioning

`MediaPlayback` is Core's only stable, engine-neutral media-session abstraction. It defines immutable snapshots, the time domain, capabilities, and cancellable commands that Media controls consume.

## Status

`in-progress / lab / P0`. The implemented contract still requires integration verification with Media controls and adapters before review.

## Scope

Includes `IMediaPlaybackSession`, `MediaPlaybackSnapshot`, `MediaPlaybackTimeRange`, command results, and revision events; excludes any player, XAML control, media source, file path, window, or network service.

## Documents

- [Specification](SPEC.en-US.md)
- [Design](DESIGN.en-US.md)
- [Integration](INTEGRATION.en-US.md)
- [Acceptance](ACCEPTANCE.en-US.md)

## Ownership

This work item exclusively owns `contracts/MediaPlayback`, Core's `MediaPlayback` source, and its Core tests. Feature controls must not duplicate public models.
