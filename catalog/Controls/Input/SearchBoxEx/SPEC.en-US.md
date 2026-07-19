# SearchBoxEx Specification

## Goal

Provide a privacy-first search input that owns text, optional in-memory history, cancellable provider suggestions, debounce, submission, and keyboard focus. Query execution, data access, and persistence remain host-owned.

## Non-goals

The control does not access file/account history, use the network, execute host queries, or deliver suggestions after unload.

## Public API

`Text`, `PlaceholderText`, `Debounce=250ms`, `MinimumPrefixLength=1`, `IsHistoryEnabled=false`, `QueryCommand`, `SuggestionProvider`, `SearchState`, `IsSuggestionListOpen`, `Suggestions`, and `SearchHistory`. `RefreshSuggestionsAsync` cancels the previous request and rejects late results by version. `SubmitAsync` raises `QuerySubmitted` and may invoke `QueryCommand`; `ClearHistory` only clears in-memory history.

## State and input

States are `Idle/Loading/Results/Error`. Once the minimum prefix is met, the provider runs after Debounce; with no provider the control can fall back to opt-in in-memory history. Down moves focus to suggestions, Enter raises suggestion invocation then submits, Escape closes, and F6 focuses search.

## Template and lifecycle

`PART_EditBox` is required; `PART_SuggestionList` and `PART_ProgressRing` are optional. Missing optional parts do not disable API use. Unload stops debounce, cancels the provider, increments the request version, and stops progress; replacing the provider cancels the old request and schedules the current query.

## Failure and accessibility

Provider exceptions enter Error and raise `SearchError`; a handler can suppress the default HelpText. The suggestion list uses a polite live setting with stable text and visible focus. Chinese, English, RTL, high contrast, and Reduced Motion do not change query semantics.

## Current boundary

The item remains `in-progress/lab/P1`; trend search, voice input, and persisted history require separate review.
