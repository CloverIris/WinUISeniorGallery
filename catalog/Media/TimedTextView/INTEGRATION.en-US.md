# TimedTextView Integration

## Global Contracts

Model types follow `contracts/TimedText` and positions follow `contracts/MediaPlayback`. The control uses Theme, Motion, Input, Accessibility, Localization, and Resources contracts. Public models have one owner; the Media project never duplicates Foundation types.

## Data Flow

The control creates no Window/AppWindow and never moves its `ItemsRepeater`, presenters, or realized containers across XamlRoots. For multiwindow captions, the host creates one View per window and shares immutable `TimedTextDocument` snapshots; each window independently owns position, scroll, focus, and LiveRegion lifecycle.

A file parser or ASR Provider produces a new immutable `TimedTextDocument` snapshot. The View validates Revision, normalizes the active Track, and builds ID/time indexes. Playback writes `Position`; the View updates only active and realized virtualized items. When a user invokes a Segment, the host decides whether to Seek.

## ASR and Translation Providers

P0 depends only on the conceptual future `Captions.Abstractions` interface: a Provider outputs standard document snapshots, cancellation, and error state and never manipulates the View. Windows, Azure, local-model, and other Providers belong in separate packages. Machine translation and control i18n remain strictly separate; API keys, regions, and content policy belong to the host.

## Threading and Lifecycle

Providers may build snapshots in the background, but setting `Document` dispatches to the UI DispatcherQueue. Optional indexes are prebuilt off-thread and atomically replaced on UI. Unload cancels auto-scroll and pending layout and retains no Provider reference; reload restores from latest Document/Position.

## Permissions, Privacy, and Degradation

The control requests no microphone or network capability and sends no telemetry. Providers obtain host-managed consent and disclose data destinations. Missing translation falls back to original; missing word timing highlights the Segment; missing language uses UI direction while Automation reports language unknown.

## Resources and Formatting

Visible labels, empty states, Automation states, and time labels are localized. Text content is displayed literally with XAML-safe escaping and never interpreted as Markdown/HTML. Fonts are app-overridable; do not bundle fonts or subtitle assets of unknown provenance.
