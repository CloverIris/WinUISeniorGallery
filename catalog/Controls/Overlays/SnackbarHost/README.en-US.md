# SnackbarHost

`SnackbarHost` owns transient presentation and an independent queue for one window. `ISnackbarService` provides explicit target binding, priority, cancellation, and deduplication. Messages never migrate across windows.

## Status

- Work item: `controls.snackbar-host`
- Maturity: Lab; priority: P0; implementation status: Ready
- Package: `WinUI3.Senior.Controls`; Gallery: `/controls/snackbar-host`

## Dependencies and ownership

Consumes theme, motion, and accessibility contracts. An implementation agent may modify only `feature.json.owned_paths`; missing shared contracts make the item `blocked`.

## Documents

[Specification](SPEC.en-US.md) · [Design](DESIGN.en-US.md) · [Integration](INTEGRATION.en-US.md) · [Acceptance](ACCEPTANCE.en-US.md). Chinese is normative and English is synchronized.
