# MediaTimeline Integration

## Global Contracts

Use the unified time domain and playback semantics from `contracts/MediaPlayback`, plus Theme, Motion, Input, Accessibility, Localization, and Resources contracts. `BufferedRanges` and `DisabledRanges` consume `WinUI3.Senior.Core.MediaPlaybackTimeRange`; MediaTimeline owns no second public range type. `MediaPlayerChrome` may compose this control; `MediaTimeline` never depends back on Chrome.

## Data Flow

The control belongs to the calling page's current XamlRoot. It creates no Window/AppWindow, changes no Presenter, and shares no pointer capture across windows. If one playback session appears in multiple windows, each window creates its own `MediaTimeline` and the host synchronizes properties through shared session snapshots. Closing one window cancels only that window's scrub and subscriptions.

The host writes `Minimum`, `Maximum`, `Position`, `PlaybackRate`, and collection snapshots from its playback session. User input produces throttled preview events; release or keyboard action produces a Seek request; the host performs Seek and then updates `Position`. This one-way loop prevents false success. Rate changes remain owned by the playback session or `MediaPlayerChrome`; the timeline only presents the result.

Live DVR updates submit range and position in one DispatcherQueue batch. `LiveWindowEndTime` is the absolute time corresponding to `Maximum`; other wall-clock labels derive from the delta.

## Threading and Lifecycle

Public properties are set only on the UI thread; background collections become immutable snapshots first. The control does not subscribe to mutable collection internals. Unload stops throttle timing and pointer capture; reload rebuilds lightweight visuals from current properties.

## Platform APIs and Permissions

The control does not call media, network, storage, or window APIs and requires no app capability. It may draw with XAML/Composition and never depends on a concrete media player.

## Degradation and Localization

Missing chapters, markers, or buffering simply leave those layers empty. Missing wall-clock anchor shows relative time. Time formatting follows locale and content beyond 24 hours does not wrap; Automation text is localized while numeric range values remain media seconds.
