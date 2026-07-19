# FontPicker Specification

## Goal

Provide a host-fed local font selector with search, favorites, preview, and selection notifications. Font enumeration, installation, registry access, and persistence remain host responsibilities.

## Non-goals

The control does not install fonts, read the registry, load web fonts, persist favorites, or claim that a font file is available.

## Public API

- `SetOptions(IEnumerable<FontPickerOption>)`: replaces the list and de-duplicates by case-insensitive `FamilyName`; stale favorites and selection are removed.
- `Options`, `VisibleOptions`, `FavoriteFamilies`, and `SelectedOption`: read-only views.
- `SelectedFontFamily`: bindable string; a valid external change raises a non-user `SelectionChanged` event. Unknown names are retained but do not raise selection.
- `SearchText`, `IsFavoritesOnly`, `IsPreviewEnabled`, and `PreviewText`: filter and preview state.
- `Select(string, bool isUserInitiated = true)`: returns `false` for an unknown name and raises `SelectionChanged` on success.
- `ToggleFavorite(string)` and `IsFavorite(string)`: operate only on known options and raise `FavoritesChanged` plus option-change notifications.

`FontPickerOption.FamilyName` and `DisplayName` are required. `Category` is optional. APIs are UI-thread APIs and events are raised in committed state order.

## State and filtering

Filtering applies the favorites-only predicate first, then a current-culture case-insensitive match over FamilyName, DisplayName, and the search query. `VisibleOptions` is rebuilt immediately after `SetOptions`. Removing the selected option clears `SelectedFontFamily`.

## Template and degradation

The template may provide search, option list, favorite toggle, and preview parts. Missing optional parts do not disable API use. The control does not verify that a font is installed; a host may label unavailable options through `DisplayName` or `Category`.

## Failure modes

An empty list displays an empty state; duplicate families collapse to the first option; unknown selection returns `false`; search does not throw. Unloading does not cancel font enumeration because the control never starts enumeration itself.

## Current boundary

The item remains `in-progress/lab/P2`. System enumeration, variable axes, recent fonts, and installation require a separate review and must not be invented inside this control.
