# TimedTextView

## Positioning

`TimedTextView` is a unified timed-text control whose Document → Track → Segment → Word model covers single-line captions, scrolling lyrics, word-level Karaoke, bilingual text, and live ASR revisions.

## Scope

- Includes time matching, incremental snapshots, track selection, virtualized rendering, auto-scroll, and original/translated presentation.
- Excludes speech recognition, machine translation, subtitle parsing, media playback, and Provider credential management.

## Status

- Work item: `ready`
- Maturity: `lab`
- Priority: `P0`
- Package: `WinUI3.Senior.Media`

## Dependencies

- `foundation.timed-text`
- `foundation.media-playback`

## Documents

- [Specification](SPEC.en-US.md)
- [Design](DESIGN.en-US.md)
- [Integration](INTEGRATION.en-US.md)
- [Acceptance](ACCEPTANCE.en-US.md)

## Agent Ownership

An implementation agent may change only `feature.json` `owned_paths`. Providers integrate only through global abstractions; never add a concrete cloud-service dependency to the control.
