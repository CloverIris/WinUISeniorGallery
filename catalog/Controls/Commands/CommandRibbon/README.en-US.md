# CommandRibbon

`CommandRibbon` is a host-fed contextual command strip. The control owns tabs, groups, key tips, responsive density, and deterministic overflow ordering; command side effects remain in the host's `ICommand` implementations.

## Status

in-progress / lab / P2.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Controls/Commands/CommandRibbon

Implementation: `src/WinUI3.Senior.Controls/Commands/CommandRibbon`

## Implementation readiness
The current implementation includes `CommandRibbon`, `CommandRibbonTab`, `CommandRibbonGroup`, and `CommandRibbonCommand`; it supports `AlwaysExpanded`, `AlwaysMinimized`, and `Auto` collapse policies, stable priority-based overflow, unique KeyTip invocation, disabled-command protection, and observable layout results. It creates no windows, persists no commands, and uses no Office assets. Gallery integration and automated verification remain follow-up work.
