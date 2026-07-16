# ArchaeologyIndexPage Integration

## Global contracts

- contract.architecture.v1: Gallery is composition root.
- contract.navigation.v1: route validation and Back restoration.
- contract.localization.v1: strings are resources and RTL is supported.
- contract.resources.v1: assets have one owner and license record.
- contract.theme.v1: semantic ThemeResource values only.
- contract.input.v1: equivalent keyboard/touch/pointer commands.
- contract.accessibility.v1: Automation, scaling, High Contrast, Reduced Motion.

## Services and data flow

- Input: Archaeology Catalog + SOURCES + license manifest.
- Request one immutable read-only snapshot per load/refresh.
- Snapshot acquisition accepts a cancellation token.
- Parsing, sorting, filtering, and grouping avoid XAML objects.
- UI applies the latest complete snapshot on its Dispatcher.
- Services are scoped to the owning window.
- Commands return success, rejection, cancellation, or failure.
- No process-wide mutable static state stores page data.

## Route and lifecycle

- Register `/archaeology` as `route.gallery.archaeology` exactly once.
- Accept only `period, product, source, license`.
- OnNavigatedTo validates route and restores view state.
- OnNavigatedFrom stores normalized parameters, selection, and anchor.
- Unloaded releases events, timers, instances, and transient surfaces.
- Obsolete load generations are discarded.
- Window close prevents cross-window Snackbar/navigation delivery.
- External links require explicit activation.

## Resources and localization

- UI text and Automation names use localization keys.
- IDs, API names, routes, resource keys, and event names remain English.
- Missing locale falls back to en-US and emits non-fatal diagnostics.
- Assets resolve only through the asset manifest.
- Unapproved or missing assets use placeholders.
- Dates, counts, durations, and versions use correct locale semantics.
- Historical Microsoft assets require approved license records.

## Privacy and security

- download no remote historical asset and request no account.
- Never log query text, media, captions, contacts, paths, credentials, or legal selections.
- Route parameters are untrusted input.
- Clipboard output uses a documented privacy allow list.
- URI Launcher displays destination domain before launch.
- No unregistered preview is downloaded.
- No arbitrary type, assembly, XAML, or executable content is loaded.

## Performance budget

- First interactive local frame target: 300 ms; About core legal text: 200 ms.
- UI synchronous work over 50 ms emits a performance event.
- Lists/grids virtualize at 200+ entries; Controls target is 1,000.
- Images decode near render size and leave with realization buffer.
- Search/filter is cancellable and latest-wins.
- Skeleton delay is 100 ms to avoid warm-cache flash.
- Stable unchanged cards are not rebuilt on refresh.
- Event logs are bounded and updated in batches.

## Observability

- Event prefix: `gallery.archaeology`.
- `gallery.archaeology.load_started`: route and catalog version.
- `gallery.archaeology.load_completed`: duration and valid/invalid counts.
- `gallery.archaeology.state_changed`: previous/next state and reason enum.
- `gallery.archaeology.navigation_requested`: target feature ID and result.
- `gallery.archaeology.error`: stable code and correlation ID only.
- Events contain no user content and obey host diagnostics policy.
- Counters are window-scoped and disposed with the page.

## Fallbacks

- Empty: retain the non-affiliation notice, research method, and filter summary.
- Error: show Retry, Return home, and correlation ID.
- Permission/capability: download no remote historical asset and request no account.
- Offline: use packaged catalog, text, and approved local assets.
- Unknown maturity: show Unknown and disable Run.
- Unsupported capability remains discoverable with guidance.
- Storage unavailable preserves session state only.
- Localization failure uses en-US and stable IDs.
