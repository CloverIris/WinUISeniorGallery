# WinUI 3 Senior Gallery Product Requirements Document

**Version**: 0.1 (specification baseline)  
**Normative language**: Simplified Chinese. This English document is synchronized translation and must not reverse-change APIs or product decisions.  
**Product state**: Pre-implementation / specification-first  
**Related material**: [Catalog](roadmap/FEATURE-CATALOG.md) · [Architecture](architecture/README.md) · [Agent contract](../AGENTS.md)

## 1. Product definition

WinUI 3 Senior Gallery is two mutually supporting products:

1. A set of modular WinUI 3 advanced composite controls and experience compositions that ordinary WinUI applications can consume.
2. A packaged MSIX Gallery that is both a discovery/demo app and the visual validation surface for interaction, accessibility, theme, input, and performance contracts.

“Advanced” means a capability handles several of content structure, state, input, visuals/motion, accessibility, and failure degradation. It is not merely a reskin of an atomic control. Components retain host business autonomy: they do not silently own networking, accounts, content rights, a playback engine, window creation, or cloud-service policy.

## 2. Context and opportunity

WinUI 3 Gallery focuses on basic controls and platform capabilities; commercial suites usually focus on enterprise data entry, grids, and charts; Community Toolkit intentionally retains a lighter general-purpose boundary. Consumer media, content, desktop-workbench, and immersive apps therefore repeatedly rebuild virtualized carousels, home content rails, player chrome, floating controls, Live DVR timelines, lyrics/captions, draggable panels, notification queues, tear-out tabs, and focused ten-foot layouts.

The project does not claim that no implementation exists. It addresses the lack of an open WinUI 3 baseline that is simultaneously Fluent-integrated, fully testable in API/template/input/accessibility/performance terms, selectively consumable, historically traceable, and discussable through a running Gallery.

## 3. Users and jobs to be done

| User | Primary job | Success criterion |
| --- | --- | --- |
| WinUI application developer | Integrate quality media/content/window UI in hours instead of rebuilding interaction foundations for weeks. | References only required packages, replaces templates, and remains decoupled from a playback/data service. |
| Designer or product owner | Evaluate real interaction states, failures, and accessibility rather than static screenshots. | Gallery switches theme, input, size, RTL, and Reduced Motion. |
| Component contributor | Claim a bounded work item and deliver an implementation without specification drift. | Chinese spec, API, visual tree, acceptance, and `owned_paths` are sufficient for independent work. |
| Design-history researcher | Turn valuable classic Windows patterns into discussable modern patterns. | Sources are traceable, attribution is accurate, and protected assets are absent. |

## 4. Vision, principles, and non-goals

### 4.1 Vision

Become the credible open-source baseline for advanced modern Windows consumer interactions: directly usable, deeply customizable, accessible, and maintainable over time.

### 4.2 Principles

- **Specification over screenshot**: implementations explain state, boundaries, failure, performance, and accessibility.
- **Composite capabilities on atomic platform primitives**: reuse WinUI/Windows App SDK rather than rebuild the platform.
- **Host control is preserved**: media UI uses an abstract playback session; window UI requests or receives from a host; P0 providers do not smuggle in cloud services.
- **Per-window isolation**: messages, focus, lifecycle, and overlays must not leak across windows by default.
- **Progressive stability**: `lab → preview → stable` is a quality promise, not marketing.
- **Historically grounded, modernly maintainable**: preserve interaction DNA, not proprietary appearance.

### 4.3 Non-goals

- Replacing general enterprise admin, ERP, DataGrid/report/Gantt suites.
- Providing account, sync, CDN, ASR, translation, or media-decoding backends.
- Turning Gallery into a business app, content platform, or telemetry collector.
- Promising pixel-perfect recreation of Windows, Zune, Xbox, Office, or any Microsoft product.
- Implementing the existing C++ Canvas core in the first round; only its reviewable abstraction/integration boundary is reserved.

## 5. Scope and module boundaries

| Module | Owns | Must not own |
| --- | --- | --- |
| `Core` | Theme, motion, input, accessibility, navigation, localization, resources, cross-module contracts. | Reverse dependency on Gallery, archaeology, a concrete player, or business services. |
| `Controls` | Reusable advanced controls such as Carousel, BottomSheet, pickers, rails, and grids. | App navigation, networking, or business data sources. |
| `Media` | Player chrome, timeline, timed text, mini player, media layouts. | Binding to one player engine; P0 creation/migration of windows. |
| `Windowing` | Title bar, floating host, edge/settings panels, dock preview, window behaviors. | Implicit takeover of process-level window policy. |
| `Archaeology` | Experience compositions, design archaeology, source management. | Stable APIs that conflict with Controls/Media/Windowing. |
| `Gallery` | Pages, navigation, examples, configuration, validation scenarios. | A dependency of other modules or a business backend. |

The fixed graph is `Controls → Core`; `Media → Core + Controls`; `Windowing → Core`; `Archaeology → Core + Controls + Media + Windowing`; `Gallery → all modules`.

## 6. Information architecture and Gallery-page requirements

Gallery is a component catalog, interaction lab, and design handbook rather than a simple sample list. Stable routes are `/`, `/controls`, `/media`, `/windowing`, `/experiences`, `/archaeology`, `/playground`, `/settings`, and `/about`.

Each page specification must define visual tree, section ordering, data model, loading/empty/error states, deep linking, back behavior, narrow/medium/wide layout, keyboard focus order, Narrator names, automation IDs, theme/RTL/text-scaling behavior, and cancellation after page unload.

## 7. Functional requirements

### FR-01 Catalog and work items

Each capability declares ID, status, maturity, priority, package, route, dependencies, provisions, archaeology references, ownership, and license review in `feature.json`. It owns bilingual README/SPEC/DESIGN/INTEGRATION/ACCEPTANCE; archaeology adds SOURCES. A `ready` specification has no Open Decisions; a `proposed`/`specified` item declares its closing conditions. Catalog validation checks JSON shape, documents, unique IDs/routes/provisions/owned paths, reachable dependencies, and cycles.

### FR-02 Theme, motion, input, and accessibility

Formal components support Light, Dark, and High Contrast through ThemeResource/contracts rather than hard-coded colors. Time-based motion has a Reduced Motion path and cannot be required for functionality. Mouse, touch, precision touchpad, keyboard, and where applicable controller have equivalent core paths. Accessibility baseline includes AutomationPeer, visible focus, semantic name/value, live regions, 100/150/200% DPI, text scaling, Chinese/English, and RTL.

### FR-03 Advanced controls

The initial catalog includes Carousel/Pivot/Semantic Zoom; Breadcrumb/Search/Wizard; Adaptive Grid/Content Rail/Widget/Dynamic Tile; Bottom Sheet/Snackbar/Overlay; command surfaces; pickers; Property Grid; and Tree Data Grid. Before `ready`, every control defines public API, state, template parts, data/virtualization strategy, input matrix, boundary errors, and acceptance budgets.

### FR-04 Media

`IMediaPlaybackSession` is the stable player-chrome abstraction, with a planned MediaPlayerElement adapter but no binding to it. Timeline supports VOD/Live DVR, buffered/disabled/chapter/marker ranges, throttled preview, final seek, Live Edge, and accessible Value semantics. Timed Text uses Document → Track → Segment → Word for captions, lyrics, karaoke, bilingual display, and ASR revisions. P0 represents full-screen/floating requests and status only; it does not create, move, or migrate windows.

### FR-05 Windowing and overlays

Windowing owns chrome host, compact overlay, floating widgets, edge commands, settings panel, dock preview, tab tear-out, reveal border, and connected transitions. Every item declares host-provided window handle/lifecycle, degradation, focus ownership, cross-window prohibitions, and platform-version differences.

### FR-06 Experience compositions and placeholders

Home Screen, Hub Panorama, Immersive Now Playing, Widgets Board, Tabbed Shell, Focus Session, Game Bar, Immersive Reader, Editor Canvas, and Captions Translation compose stable capabilities and do not take ownership of their underlying APIs. `Captions.Abstractions`, ASR/translation providers, and Canvas abstractions are explicit future placeholders with no default network/cloud/proprietary implementation.

### FR-07 Design archaeology

Every exhibit records original product, version/era, source URL, access date, copyright/asset state, retained design DNA, modernization changes, modern feature references, and non-goals. Historical assets stay out of the repository unless an exceptional future use passes independent license review.

## 8. P0 definition of done

| P0 | Delivery boundary | Quality threshold |
| --- | --- | --- |
| CarouselView | Virtualization, multi-input, autoplay, bounded/looping navigation, three transitions. | 1,000 logical items are not all realized; Reduced Motion, RTL, and AutomationPeer are complete. |
| BottomSheet | Modal/non-modal, snap points, drag close, scrim, Esc, focus, size fallback. | Every close route is consistent; narrow enters from bottom and wide degrades per specification. |
| SnackbarHost | Per-window host/queue, priority, dedupe, action, cancellation, announcement. | Window destruction cancels hidden items and never delivers to another window. |
| MediaPlayerChrome | Play/pause/stop/seek/volume/rate/full-screen/floating requests. | Depends only on playback-session abstraction; P0 does not create windows. |
| MediaTimeline | VOD/Live DVR, ranges, and throttled preview. | Drag emits throttled preview, release sends one seek, keyboard immediately synchronizes Automation value. |
| TimedTextView | Captions, lyrics, karaoke, bilingual, incremental ASR. | Model supports incremental revision; P0 has no ASR/translation provider implementation. |

Directory-level Chinese SPEC/DESIGN/INTEGRATION/ACCEPTANCE govern exact APIs, templates, and tests; this PRD does not override them.

## 9. Non-functional requirements

### 9.1 Performance

Repeatable content declares virtualization/recycling. Input cannot be blocked by network, decoding, or long animation; async work is cancellable. Effects prefer Composition/platform support over high-frequency XAML layout. Every ready item provides an executable instantiation, scroll, input, or update budget.

### 9.2 Reliability and lifecycle

Subscriptions and timers release on unload/window destruction, UI-thread responsibility is explicit, and empty/replacement/rejected/missing-capability/missing-template cases are deterministic. Host destruction must cancel pending callbacks rather than redirecting them to another host or window.

### 9.3 Privacy and security

No telemetry, media content, speech, text, or usage behavior is collected by default. Future external providers are explicitly configured by the host and declare network, credentials, data residency, and cancellation. Security reporting follows `SECURITY.md`; Gallery contains no real user content or secrets.

### 9.4 Compatibility and maintainability

Minimum system is Windows 10 1809; later APIs need checks or degradation. Public APIs follow semantic-version compatibility review and visible strings are resource-backed. Stable terminology and APIs do not change with translation.

## 10. Quality gates and acceptance

`proposed → specified` requires goals, scope, dependencies, and initial interactions. `specified → ready` requires locked Chinese API/defaults/state/template/failure/performance budget; synchronized English heading/API/ID/scenarios; explicit contracts; Given/When/Then for normal, empty, error, cancellation, theme, High Contrast, RTL, DPI, keyboard, touch, Narrator, and Reduced Motion; and passing JSON/link/ownership/dependency/architecture gates. Implementation then requires Gallery demo, unit/architecture tests, necessary UI/visual tests, and performance evidence before `review`/`done`.

## 11. Success measures

Public preview succeeds when the six P0 items can be independently referenced by a new WinUI app with a minimal sample and Gallery scenario; they have no known blocking defects on the baseline matrix; consumers do not acquire Gallery or unrelated modules; every public capability links from Gallery to specification/acceptance/sources where applicable; and an external contributor can complete a directory-scoped item without changing central configuration.

## 12. Risks and open product issues

Scope growth is contained by catalog status and `ready` gates. Platform evolution is isolated by contracts and Gallery degradation checks. Media/ASR complexity remains behind sessions/providers. Per-window hosts and cancellation prevent window leaks. SOURCES, `license_review`, and the no-assets rule control archaeology attribution/copyright risk. Chinese source, bilingual structure tests, ownership, and Gallery scenarios control documentation drift.

## 13. Release and governance

Packages follow the meta-package plus sub-package approach: Core, Controls, Media, Windowing, Archaeology, and future All. `lab` evolves quickly, `preview` has migration notes, and `stable` follows semantic-version compatibility. Public release requires license/notice/package metadata/sample/changelog/security review. The repository is MIT-licensed but grants no extra rights in third-party product names, trademarks, or historical assets, and is not affiliated with, endorsed by, or sponsored by Microsoft.
