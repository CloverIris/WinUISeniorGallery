# Resources Contract

## Contract identity

- Contract ID: `contract.resources.v1`
- Dependencies: contract.architecture.v1
- Normative keywords MUST, MUST NOT, SHOULD, and MAY are binding at their stated strength.
- Chinese zh-CN is the normative source; en-US mirrors heading depth, IDs, APIs, keys, and event names.

## Scope

Defines resource keys, ownership, merge order, conflicts, asset manifest, and licensing.
- Applies to production controls, experiences, Gallery integration, adapters, and tests that consume this contract.
- Does not transfer ownership of feature-specific public APIs to the contract package.

## Terms

- `PublicKey`: Cross-module resource identity.
- `PrivateKey`: Feature-internal resource.
- `AssetId`: Manifest-registered asset.
- `ResourceOwner`: Single provider.

## Responsible parties

- Contract owner MUST maintain stable semantics, compatibility notes, and conformance tests.
- Provider MUST validate inputs, honor cancellation, publish documented results, and release scoped resources.
- Consumer MUST depend on the abstraction rather than inspect provider implementation types.
- Gallery MUST use local deterministic data unless the user explicitly configures an external capability.
- Feature owner MUST document deviations and remain blocked until an incompatible contract gap is resolved.

## Normative rules

- Public keys use Senior.<Area>.<Name>; private keys use Senior.Internal.<FeatureId>.<Name>.
- A public key has exactly one provides owner.
- Overrides use documented theme extension points, never accidental dictionary order.
- Binary assets record source, license, hash, purpose, and dimensions.
- Unlicensed historical Windows material is never packaged.
- Resource resolution never downloads from the network.

- Every externally visible state MUST have a deterministic fallback.
- Every asynchronous operation MUST support cancellation or document why completion is bounded.
- Diagnostics MUST use stable codes and MUST NOT include user content.

## Minimum surface

### Interfaces and models

- `IResourceCatalog`: 资产解析.
- `ResourceResolutionFailed`: 冲突诊断.
- `Senior.<Area>.<Name>`: Public-key naming.

- Public values MUST be immutable snapshots or explicitly documented commands.
- Commands MUST return an observable result: success, rejected, cancelled, or failed.
- Stable identities MUST compare by value and MUST NOT rely on object reference identity.

### Resources and events

- Public resource keys MUST follow the Resources Contract and have one owner.
- Events MUST carry a monotonic revision or generation when stale delivery is possible.
- Event delivery MUST be ordered within its documented scope.
- Subscriptions MUST be releasable and MUST NOT keep a window alive.
- Event names, API names, stable IDs, and resource keys remain English in every locale.

## Threading and window scope

IResourceCatalog is process-read-only; UI resources instantiate on the caller window Dispatcher and binary streams share no mutable window state.
- UI objects MUST be created, read, and mutated only on their owning Dispatcher.
- Background work MAY process immutable data and MUST check cancellation before publication.
- A closed window MUST NOT receive Navigation, Snackbar, Automation, or visual updates.
- Cross-window messages MUST contain stable IDs and immutable data, never UIElement, XamlRoot, or routed-event args.
- Latest-wins consumers MUST reject stale revision/generation values.

## Errors and fallback

Missing resources use documented system/semantic placeholders; duplicate keys, hash mismatch, and blocked licenses fail validation.
- Rejected means the request is valid but unsupported by current policy/capability.
- Cancelled is not logged as an operational failure unless cancellation cleanup fails.
- Failed results MUST include a stable error code; correlation IDs MUST be content-free.
- Fallback MUST preserve core readability and keyboard operation.
- Repeated equivalent errors SHOULD be coalesced to avoid diagnostic and LiveRegion spam.

## Security and privacy

- Inputs from routes, providers, manifests, and persisted settings are untrusted until validated.
- Credentials, personal content, file paths, media, captions, and contacts MUST NOT enter default telemetry.
- External links and privileged platform actions require explicit user activation.
- Assets MUST pass license review before packaging.
- Local fake data is the default for Gallery demonstrations.

## Versioning and compatibility

Adding keys/assets is compatible; deleting/redefining public keys needs aliases, deprecation, and v2.
- Contract ID remains stable for backward-compatible additions.
- New optional fields MUST have deterministic defaults for older consumers.
- Deprecation MUST identify replacement, migration, and minimum support window.
- A v1 provider MUST reject unknown required behavior rather than silently approximate it.
- Package version and Contract version are related but not interchangeable.

## Conformance matrix

- undefined: MUST have deterministic automated evidence.
- undefined: MUST have deterministic automated evidence.
- 许可: MUST have deterministic automated evidence.
- undefined: MUST have deterministic automated evidence.
- Success, rejected, cancelled, failed, and fallback paths MUST be covered.
- Tests MUST cover Light, Dark, High Contrast, RTL, Reduced Motion, keyboard, and screen reader where UI applies.
- Tests MUST cover window close and stale asynchronous delivery.
- Performance tests MUST report median and P95 with deterministic local data.
- No conformance test may require network, sign-in, or unlicensed historical assets.

## Acceptance gate

- The provider and at least one consumer compile against the contract without implementation-type leakage.
- Architecture validation finds no reverse dependency, duplicate owner, cycle, or overlapping ownership.
- Automated tests prove documented fallback and cleanup behavior.
- Chinese and English heading-depth sequences, IDs, APIs, and resource keys match.
- Accessibility and privacy review findings are resolved or explicitly blocking.
- Breaking ambiguity blocks status advancement until the contract owner resolves it.

## Prohibited patterns

- Process-wide mutable UI state.
- Fire-and-forget work without bounded lifetime and diagnostics.
- Silent cross-window rerouting after the target window closes.
- Hard-coded theme colors or user-facing natural-language concatenation.
- Vendor SDK types in stable public models.
- Treating a Gallery simulation as a Windows system capability.
