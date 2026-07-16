# WindowingIndexPage Integration

## 全局合同

- contract.architecture.v1: Gallery is composition root.
- contract.navigation.v1: route validation and Back restoration.
- contract.localization.v1: strings are resources and RTL is supported.
- contract.resources.v1: assets have one owner and license record.
- contract.theme.v1: semantic ThemeResource values only.
- contract.input.v1: equivalent keyboard/touch/pointer commands.
- contract.accessibility.v1: Automation, scaling, High Contrast, Reduced Motion.

## 服务与数据流

- Input: Windowing catalog + current-window capability snapshot.
- Request one immutable read-only snapshot per load/refresh.
- Snapshot acquisition accepts a cancellation token.
- Parsing, sorting, filtering, and grouping avoid XAML objects.
- UI applies the latest complete snapshot on its Dispatcher.
- Services are scoped to the owning window.
- Commands return success, rejection, cancellation, or failure.
- No process-wide mutable static state stores page data.

## 路由与生命周期

- Register `/windowing` as `route.gallery.windowing` exactly once.
- Accept only `feature, support`.
- OnNavigatedTo validates route and restores view state.
- OnNavigatedFrom stores normalized parameters, selection, and anchor.
- Unloaded releases events, timers, instances, and transient surfaces.
- Obsolete load generations are discarded.
- Window close prevents cross-window Snackbar/navigation delivery.
- External links require explicit activation.

## 资源与本地化

- UI text and Automation names use localization keys.
- IDs, API names, routes, resource keys, and event names remain English.
- Missing locale falls back to en-US and emits non-fatal diagnostics.
- Assets resolve only through the asset manifest.
- Unapproved or missing assets use placeholders.
- Dates, counts, durations, and versions use correct locale semantics.
- Historical Microsoft assets require approved license records.

## 隐私与安全

- 不枚举其他进程窗口或请求系统权限.
- Never log query text, media, captions, contacts, paths, credentials, or legal selections.
- Route parameters are untrusted input.
- Clipboard output uses a documented privacy allow list.
- URI Launcher displays destination domain before launch.
- No unregistered preview is downloaded.
- No arbitrary type, assembly, XAML, or executable content is loaded.

## 性能预算

- First interactive local frame target: 300 ms; About core legal text: 200 ms.
- UI synchronous work over 50 ms emits a performance event.
- Lists/grids virtualize at 200+ entries; Controls target is 1,000.
- Images decode near render size and leave with realization buffer.
- Search/filter is cancellable and latest-wins.
- Skeleton delay is 100 ms to avoid warm-cache flash.
- Stable unchanged cards are not rebuilt on refresh.
- Event logs are bounded and updated in batches.

## 可观测性

- Event prefix: `gallery.windowing`.
- `gallery.windowing.load_started`: route and catalog version.
- `gallery.windowing.load_completed`: duration and valid/invalid counts.
- `gallery.windowing.state_changed`: previous/next state and reason enum.
- `gallery.windowing.navigation_requested`: target feature ID and result.
- `gallery.windowing.error`: stable code and correlation ID only.
- Events contain no user content and obey host diagnostics policy.
- Counters are window-scoped and disposed with the page.

## 降级

- Empty: 保留能力摘要且不创建额外窗口.
- Error: show Retry, Return home, and correlation ID.
- Permission/capability: 不枚举其他进程窗口或请求系统权限.
- Offline: use packaged catalog, text, and approved local assets.
- Unknown maturity: show Unknown and disable Run.
- Unsupported capability remains discoverable with guidance.
- Storage unavailable preserves session state only.
- Localization failure uses en-US and stable IDs.
