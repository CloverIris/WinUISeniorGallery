# MediaTimeline

## Positioning

`MediaTimeline` is a playback-engine-independent timeline for VOD, pure live, and Live DVR windows, including buffered ranges, chapters, markers, disabled ranges, and Live Edge.

## Scope

- Includes rendering, scrub preview, final Seek requests, keyboard stepping, and chapter/range visualization.
- Excludes executing Seek, generating video thumbnails, loading chapter metadata, or managing playback rate.

## Status

- Work item: `ready`
- Maturity: `lab`
- Priority: `P0`
- Package: `WinUI3.Senior.Media`

## Dependencies

- `foundation.media-playback`

## Documents

- [Specification](SPEC.en-US.md)
- [Design](DESIGN.en-US.md)
- [Integration](INTEGRATION.en-US.md)
- [Acceptance](ACCEPTANCE.en-US.md)

## Agent Ownership

An implementation agent may change only `feature.json` `owned_paths`. If the time-domain or playback contract is insufficient, mark this item `blocked`; do not invent a second media-time type.
