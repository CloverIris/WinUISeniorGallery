# ContentRail Specification

## Goal

Provide a horizontal content rail owning selection, keyboard/touch direction semantics, and adjacent-peek parameters; the host owns item data, See All navigation, and business actions.

## Non-goals

No network loading, image cache, global navigation, or persisted scroll position.

## Public API

Inherits `ItemsSource`/`ItemTemplate`; exposes `Header`, `ItemWidth`, `ItemSpacing`, `PeekWidth`, `IsSnapEnabled`, `SeeAllCommand`, `PageSize`, and `IsWrapNavigationEnabled`; methods `MoveSelection`, `ScrollNext`, `ScrollPrevious`, `ScrollToIndex`, and `InvokeSeeAll`.

## State model

An empty collection has no selection. PageSize=0 moves one item; RTL mirrors Left/Right; wrapping cycles ends, otherwise boundaries clamp. Enter/Space raises ItemInvoked and SeeAllCommand receives Header.

## Template parts and visual tree

`PART_Repeater`/`PART_ScrollView` are optional theme parts; missing parts retain the ListView default layout and keyboard logic. Adjacent peeks are expressed by item-container width and spacing.

## Behavior and failure modes

Vertical wheel input bubbles to the parent; touch/Shift+wheel/direction keys use the same selection state machine. Replacing ItemsSource repairs an out-of-range selection, and unload retains no timer or event subscription.

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Proposed implementation baseline
API: `ItemsSource`, `ItemTemplate`, `Header`, `SeeAllCommand`, `ItemSpacing=12`, `PeekExtent=48`, `PageSize=Auto`; `ScrollNext/Previous`. States: `Empty/Ready/Scrolling`; parts: `PART_Repeater`, `PART_ScrollView`, optional arrows. Vertical wheel bubbles; Shift+wheel/touch/arrows move horizontally. Close Selection via keyboard-focus/UIA prototype.
