# BreadcrumbBarEx Specification

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
`ItemsSource`, `SelectedIndex`, `IsEditable=false`, `EditText`, `Suggestions`, `NavigateCommand`; states `Display/Editing/Invalid`; parts `PART_Repeater`, `PART_EditBox`, `PART_Flyout`. Enter commits, Escape cancels, overflow retains current ancestor; parse failure raises `NavigationFailed` without selection change.
