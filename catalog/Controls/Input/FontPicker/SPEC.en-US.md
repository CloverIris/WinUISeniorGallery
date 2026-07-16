# FontPicker Specification

## Goal

Define reusable responsibilities, state, and boundaries.

## Non-goals

No implementation while proposed.

## Public API

Not locked.

## State model

Not locked.

## Template parts and visual tree

Not locked.

## Behavior and failure modes

Follow referenced contracts.

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Proposed implementation baseline
`ItemsSource`, `SelectedFont`, `PreviewText`, `SearchText`, `RecentFonts`, `Favorites`; states loading/ready/empty/error; search/list/preview parts. 300 ms debounce, Enter commits, missing font retains name as Unavailable.
