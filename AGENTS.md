# WinUI 3 Senior Gallery Agent Work Contract

This file is the repository-level delivery contract for every human or agent contributor. The Chinese specifications in each work item are normative; English documents are synchronized translations. The [PRD](docs/PRD.zh-CN.md) defines product-level intent. A local `feature.json`, local Chinese `SPEC`, and referenced Contracts define the implementation-level truth.

## 1. Authority and precedence

When instructions conflict, apply this order:

1. User request and applicable security/legal constraints.
2. `AGENTS.md`, central build files, JSON Schema, and contracts.
3. The local `feature.json` and local Chinese specification.
4. The synchronized English translation, Gallery copy, samples, and implementation notes.

Do not use an old Gallery behavior, screenshot, external library, or historical product as an authority that overrides a local specification. If the specification is incomplete, mark the work item `blocked` with evidence instead of inventing a private public API.

## 2. Claiming a work item

1. Claim one complete feature directory, not a single language file.
2. Read `feature.json`, every local Chinese document, every referenced Contract, and the PRD section relevant to the feature before editing.
3. Check `owned_paths`. Modify only the assigned directory unless the maintainer explicitly assigns a shared Contract or root file.
4. Do not change central package versions, project references, solution files, JSON Schema, generated indexes, or another work item while completing a local feature.
5. Record external assumptions in local `INTEGRATION`; never hide them in source code or a commit message.

## 3. Work-item lifecycle

| Status | Meaning | Permitted work |
| --- | --- | --- |
| `proposed` | Problem/idea is cataloged but not fully designed. | Research and document; no formal implementation. |
| `specified` | Product shape is sufficiently understood but implementation contract remains open. | Deepen specification, close explicit decisions, prototype only with maintainer approval. |
| `ready` | API, behavior, integration, accessibility, performance, and acceptance are locked. | Implement, test, and review within the owned paths. |
| `in-progress` | Implementation is active. | Change implementation/tests and correct local specs when evidence requires it. |
| `blocked` | A real external/shared decision prevents progress. | Document evidence, impact, and the exact decision needed. |
| `review` | Implementation and verification are ready for maintainer review. | Resolve review feedback only. |
| `done` | Reviewed, verified, documented, and releasable for its maturity. | No behavior change without a new lifecycle pass. |

Only `ready` items may receive formal `.cs`, `.xaml`, public API, resource-key, or package implementation. A status transition must be honest: changing a label is never a substitute for closing its gate.

## 4. Required document quality

Every work item owns these bilingual pairs:

| Document | Required content |
| --- | --- |
| `README` | User value, status/maturity/priority, scope, dependency summary, document navigation, ownership, and implementation gate. |
| `SPEC` | Goals/non-goals, public API or lock conditions, defaults, state model, visual/tree/template parts, behavior, data changes, errors, and degradation. |
| `DESIGN` | Information hierarchy, layout, responsive breakpoints, visuals, interaction, motion, input, focus, accessibility, theme, RTL, and modernization tradeoffs. |
| `INTEGRATION` | Contracts, services, resource keys, platform APIs, capability/permission boundary, threading, lifecycle, cancellation, host/window boundary, privacy, and diagnostics. |
| `ACCEPTANCE` | Concrete Given/When/Then scenarios, automation hooks, performance budget, input matrix, theme/high-contrast/RTL/DPI coverage, and release gate. |

An archaeology item additionally owns bilingual `SOURCES`: product/version/era, source URL, access date, attribution, copyright/asset state, retained design DNA, rejected legacy behavior, and links to modern feature owners.

For meaningful `specified` and every `ready` item, headings alone are not sufficient. A reader must be able to answer: what appears on screen; which state it is in; which input does what; what happens when data, host, window, or platform fails; and how the behavior is tested.

## 5. API and design constraints

- Public types, resource keys, routes, stable IDs, AutomationIds, and test IDs are English and use the local naming convention.
- A public API must state owner, type, default, range/nullability, threading, event order, error behavior, and compatibility intent.
- A control template must name required versus optional `PART_*` elements, visual states, fallback behavior, and the exception/failure path for missing required parts.
- No component may silently create a window, request network access, collect telemetry, own user data, or bind a cloud/provider implementation unless its local specification explicitly authorizes it.
- Prefer host-owned interfaces and requests over global singletons. Per-window state must never be routed to an arbitrary active window.
- A Gallery page is not a public-control owner. An archaeology exhibit is not a public-control owner. They reference the stable feature that owns the API.

## 6. Cross-cutting quality baseline

Every formal component/page must state behavior for the applicable portions of:

- Light, Dark, High Contrast, Backdrop fallback, and theme change without page reconstruction.
- 100%, 150%, and 200% DPI; text scaling; narrow, regular, and wide window sizes.
- Keyboard, mouse, touch, precision touchpad, and controller where relevant.
- Focus order, Escape/back behavior, Narrator/AutomationPeer semantics, live announcements, and visible focus.
- Chinese, English, and RTL; never assume LTR geometry or fixed text width.
- Reduced Motion; animations may enhance but must not gate task completion.
- Loading, empty, replacement, failure, cancellation, host rejection, platform capability absence, and disposal/unload.

Large collections must declare realization/virtualization behavior. Async work must declare cancellation and UI-thread ownership. Performance budgets must be observable and measurable rather than aspirational.

## 7. Translation synchronization

Chinese is normative. English must preserve the same heading hierarchy, stable IDs, API names, enum names, defaults, numeric thresholds, routes, and acceptance cases. English may clarify language but may not silently introduce, remove, or reinterpret a product/API decision. Run the architecture tests before handoff; they check paired document structure and local links.

## 8. Historical research and assets

Historical products are research inputs, not asset sources. Do not commit screenshots, wallpapers, audio, icons, logos, fonts, extracted binaries, or copied UI markup without an approved license review. Do not claim a pattern was invented by a product without a source. For example, Cover Flow belongs to media-software history and must not be labelled a Zune original.

## 9. Handoff checklist

Before handoff:

1. Re-read the Chinese `SPEC`, `DESIGN`, `INTEGRATION`, and `ACCEPTANCE` as if you were a new implementer.
2. Confirm English headings/API/IDs/thresholds match Chinese.
3. Confirm all local links resolve and every `feature.json` array is a JSON array, including a one-item array.
4. Confirm dependencies point to existing Catalog IDs and do not reverse the package graph.
5. Update acceptance evidence and lifecycle status honestly.
6. Run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\eng\scripts\Validate-Catalog.ps1
dotnet test .\tests\WinUI3.Senior.Architecture.Tests\WinUI3.Senior.Architecture.Tests.csproj -c Release
```

7. If the feature is implemented, run the smallest relevant unit/UI/visual/performance test set and record the result in `ACCEPTANCE`.
