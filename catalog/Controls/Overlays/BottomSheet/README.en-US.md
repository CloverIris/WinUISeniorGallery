# BottomSheet

`BottomSheet` is an adaptive modal or modeless overlay with multiple snap points and drag-to-dismiss. It owns presentation, focus, scrim, and gestures, but not business navigation or persistence.

## Status

- Work item: `controls.bottom-sheet`
- Maturity: Lab; priority: P0; implementation status: Ready
- Package: `WinUI3.Senior.Controls`; Gallery: `/controls/bottom-sheet`

## Dependencies and ownership

Consumes theme, motion, input, and accessibility contracts. An implementation agent may modify only `feature.json.owned_paths`; missing shared contracts make the item `blocked`.

## Documents

[Specification](SPEC.en-US.md) · [Design](DESIGN.en-US.md) · [Integration](INTEGRATION.en-US.md) · [Acceptance](ACCEPTANCE.en-US.md). Chinese is normative; English is synchronized.
