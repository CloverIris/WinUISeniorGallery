# WinUI 3 Senior Gallery

> An advanced WinUI 3 component library, experience-composition library, and Windows design archaeology project for modern consumer-facing Windows apps.

[简体中文](README.zh-CN.md) · [Product Requirements Document (Chinese normative source)](docs/PRD.zh-CN.md) · [Feature Catalog](docs/roadmap/FEATURE-CATALOG.md) · [Contributing](CONTRIBUTING.md)

## The problem

WinUI 3 supplies dependable atomic controls and Fluent primitives, yet a media, content, or immersive Windows app repeatedly has to assemble carousels, home-content rails, floating playback surfaces, timelines, captions, title bars, notification queues, and ten-foot focus experiences from scratch. This project turns those recurring, reusable, interaction-sensitive compositions into selectively consumable NuGet modules, then uses a Gallery to demonstrate, compare, and validate them.

It is neither a replacement for enterprise LOB suites nor a reproduction of Microsoft products. Design archaeology identifies interaction DNA that remains valuable in historic Windows products, then redesigns it under modern Fluent, accessibility, mouse/touch/keyboard/controller, and multi-window constraints.

## Current stage

The repository is in its specification-first scaffold stage. Each directory in `catalog/` is an independently claimable work item; `feature.json` is the machine source of truth; Chinese documents are normative; only a `ready` item may enter implementation.

The first implementation-ready P0 items are:

- `controls.carousel-view`: virtualized carousel with Slide, Fade, CoverFlow, autoplay, and full input support.
- `controls.bottom-sheet`: modal/non-modal sheet with snap points, drag, focus containment, and size fallback.
- `controls.snackbar-host`: per-window message queue and service interface.
- `media.media-player-chrome`: player controls decoupled from a concrete playback engine.
- `media.media-timeline`: VOD, Live DVR, buffered ranges, chapters, and accessible timeline behavior.
- `media.timed-text-view`: lyrics, captions, karaoke, bilingual display, and incremental ASR revisions.

All other items are `proposed` or `specified`: controlled product backlog, not completed design and not implementation.

## Product form

```text
NuGet packages                         MSIX Gallery
──────────────────────────────────     ──────────────────────────────────
WinUI3.Senior.Core                     WinUI 3 Senior Gallery
WinUI3.Senior.Controls                 ├─ home and feature indexes
WinUI3.Senior.Media                    ├─ component demos and Playground
WinUI3.Senior.Windowing                ├─ theme/accessibility/input matrix
WinUI3.Senior.Archaeology              └─ archaeology and modernization notes
```

The dependency direction is fixed:

```text
Core
├─ Controls → Core
├─ Media → Core + Controls
├─ Windowing → Core
└─ Archaeology → Core + Controls + Media + Windowing

Gallery → every module
```

`Core` never depends back on Gallery, Archaeology, a concrete playback engine, or product services. P0 Media does not depend on Windowing, so an app can consume media UI without acquiring a multi-window dependency.

## Repository map

| Path | Responsibility |
| --- | --- |
| `catalog/` | Claimable feature, experience, Gallery-page, and archaeology specifications. |
| `contracts/` | Cross-module theme, motion, input, media, windowing, localization, and resource contracts. |
| `src/` | Future NuGet modules and minimal MSIX Gallery; no formal control implementation currently exists. |
| `tests/` | Architecture gates, core/control/media/windowing tests, and future UI/visual tests. |
| `eng/` | JSON Schema, templates, and catalog/theme generation and validation scripts. |
| `docs/` | PRD, architecture, governance, roadmap, and archaeology notes. |
| `samples/` | Reserved minimal consumer samples. |

Every feature directory contains `feature.json` plus bilingual README, SPEC, DESIGN, INTEGRATION, and ACCEPTANCE documents. Archaeology exhibits add bilingual `SOURCES`; they own historical research and modernization rationale only, never the stable API of a modern component.

## Specification and collaboration rules

1. Read the local Chinese specification, `feature.json`, and referenced contracts before editing.
2. One agent claims one complete feature directory; do not split translations among agents.
3. `owned_paths` is the edit boundary. Shared contracts, project dependencies, central build configuration, and generated indexes are maintainer-owned.
4. Stable IDs, public APIs, resource keys, routes, AutomationIds, and test IDs are English.
5. Before `ready`, lock API, state model, template parts, defaults, failure/degradation behavior, performance budget, and acceptance. Missing shared contracts require `blocked`, not a locally invented substitute API.
6. Every handoff updates acceptance/status and passes catalog validation.

See [AGENTS.md](AGENTS.md) for the full contract.

## Engineering baseline

- C# and .NET 10 LTS, locked through `global.json`.
- Windows App SDK 2.2 with central package management.
- Windows 10 1809 (`10.0.17763.0`) as the minimum target.
- x86, x64, and ARM64 declared; initial local Gallery verification is x64.
- Gallery is a minimal packaged WinUI MSIX app: it demonstrates components and has no product backend.
- Theme and motion flow through Theme/Motion contracts; do not hard-code brand or system theme colors.
- Light, Dark, High Contrast, 100/150/200% DPI, keyboard, mouse, touch, controller, Narrator, Reduced Motion, Chinese, English, and RTL are baseline coverage.

```powershell
dotnet restore .\WinUI3SeniorGallery.slnx -r win-x64 -p:PublishReadyToRun=true
dotnet build .\WinUI3SeniorGallery.slnx -c Release -p:PublishReadyToRun=true
powershell -NoProfile -ExecutionPolicy Bypass -File .\eng\scripts\Validate-Catalog.ps1
dotnet test .\WinUI3SeniorGallery.slnx -c Release --no-build --no-restore
```

## Design principles

- **Composite but separable**: solve real UI tasks while leaving business decisions about playback, windows, data, and networking to the host.
- **Performance is an API**: virtualization, cancellation, window isolation, non-blocking input, and Reduced Motion are contracts, not later optimizations.
- **Modernize; do not pixel-copy**: hierarchy, focus models, rhythm, and spatial ideas may be retained; original screenshots, audio, wallpaper, icons, and proprietary assets do not enter the repository.
- **Accessible by default**: keyboard/controller focus, AutomationPeer, announcement semantics, text scaling, and High Contrast are specified up front.
- **Constrain before expanding**: a small stable API is preferable to an unvalidated public promise.

## Design archaeology scope

Research covers Windows Phone/Metro, Zune, Windows Media Center, Windows 8/10/11, Xbox, Microsoft 365, Classic Windows, and media-software history. Every exhibit records its era, verifiable sources, design DNA, modernization decision, and copyright state. For example, Cover Flow is explicitly cataloged under media-software history and is not represented as a Zune original.

## Roadmap and contribution

Near-term work is to review and implement the six P0 items, with Gallery serving as their interactive contract test surface. Home Screen, Window Chrome, Widget Board, docking, Canvas, and ASR/translation abstractions progress only through `specified → ready`.

Read [CONTRIBUTING.md](CONTRIBUTING.md), [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md), and [SECURITY.md](SECURITY.md) before contributing. The project is under the [MIT License](LICENSE).

> This community-maintained project is not affiliated with, endorsed by, or sponsored by Microsoft. Windows, WinUI, Zune, Xbox, Microsoft 365, and related names/assets belong to their respective owners.
