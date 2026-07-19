# MediaPlayback Specification

## Goals and non-goals

The goal is to express playback state and commands without leaking vendor types. Loading media, selecting files, creating windows, system media controls, and presentation requests are non-goals.

## Public API

`IMediaPlaybackSession` exposes `CurrentSnapshot`, `SnapshotChanged`, `PlayAsync`, `PauseAsync`, `StopAsync`, `SeekAsync`, `SetVolumeAsync`, and `SetPlaybackRateAsync`. Every command accepts a `CancellationToken` and returns `MediaPlaybackCommandResult`: `Success`, `Rejected`, `Cancelled`, or `Failed`. `MediaPlaybackSnapshot.Revision` increases monotonically for a session and includes `PlaybackSessionId`, state, mode, capabilities, position, seekable/buffered ranges, volume, rate, and stable error information.

## State and data

States are `Unavailable`, `Loading`, `Ready`, `Playing`, `Paused`, `Buffering`, `Ended`, and `Failed`. `MediaPlaybackTimeRange` is closed; an invalid range cannot clamp a position. Snapshots and buffered collections are immutable after construction, and consumers ignore stale revisions.

## Behavior and failures

A disposed session returns `Cancelled/Disposed`; an unsupported valid command returns `Rejected/Unsupported`; invalid arguments return `Rejected/InvalidArgument`; engine failure returns `Failed/EngineFailure` without raw content or file paths. Events are delivered in order on the session UI Dispatcher.
