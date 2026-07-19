# PeopleCard Specification

## Goal

Provide a host-owned contact summary card. The control renders only explicitly supplied fields/actions and never reads contacts, accounts, or network data.

## Non-goals

No contact search, authentication, message sending, avatar download, or cross-window state.

## Public API

`PersonCardData` (Id/DisplayName/Avatar/Fields/Actions/Presence), `PeopleCardAction`, `Person`, `IsExpanded`, and `IsActionsVisible`; methods `InvokeAction` and `ToggleExpanded`; events `ActionInvoked`, `Expanded`, and `Closed`.

## State model

A null Person shows an empty card; fields preserve host order; duplicate action IDs keep the first; disabled actions raise no event; collapse hides details without discarding data.

## Template parts and visual tree

`PART_Root`, `PART_Avatar`, `PART_DisplayName`, `PART_Presence`, `PART_Fields`, and `PART_Actions` are optional; missing parts disable only their visual/action layer.

## Behavior and failure modes

Invalid Person/Action data is rejected during Normalize; hosts detach events on unload; Automation Name uses `Person {DisplayName}` and does not announce sensitive fields.

## Open Decisions

Open Decisions: focus restoration, field redaction, Avatar loading, and action permission model.

## Scenario, data, and visual tree
Person(Id,DisplayName,Avatar,Fields,Actions); tree `Anchor→OverlayMenu/Card→Actions`; Loading/Ready/Error; host policy filters fields.
