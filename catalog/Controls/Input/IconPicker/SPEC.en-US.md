# IconPicker Specification

## Goal

Provide a host-registered local icon picker with categories, favorites, recents, debounced search, code-point preview, and commit. It never scans system resources.

## Public API

`IconPickerSource` de-duplicates its `IconPickerItem` values by case-insensitive `Id`. The control exposes `SetSources`, `RegisterSource`, `SelectIcon`, `CommitSelection`, `ToggleFavorite`, `IsFavorite`, `ResolveRecent`, `RecomputeVisibleIcons`, and `Sources`, `Categories`, `Favorites`, `Recent`, `VisibleIcons`, `SelectedIcon`, `SearchText`, `SelectedCategory`, `IsFavoritesOnly`, `SearchDebounce=200ms`, and `PickerState`.

Only `IsAvailable && HasGlyph` icons are visible and committable; unknown IDs return `false`. Successful selection places the ID in an in-memory Recent list capped at 24 items; persistence is host-owned.

## Filtering and lifecycle

Filtering applies favorites, category, then Id/Name/Glyph matching. Categories are discovered from visible items without recursive mutation during filtering. Search is Dispatcher-debounced. `PART_SearchBox` is optional; APIs remain usable without it. Unload stops the search timer and late work must not overwrite a newer query.

## Input and accessibility

Enter commits the current icon and Escape clears search. Selection, commit, and state changes expose stable Automation names. High contrast, RTL, Chinese/English, and Reduced Motion do not change icon IDs or commit semantics.

## Current boundary

The item is `in-progress/lab/P2`; system Fluent enumeration, font installation, and persisted favorites require separate review.
