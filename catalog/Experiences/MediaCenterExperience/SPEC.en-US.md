# MediaCenterExperience Specification

## Goal

Compose `MediaCenterGrid` and a details overlay for Category → Item browsing with an explicit Playback intent event.

## Non-goals

No player creation, media-file access, navigation, network access, or persisted category/playback state.

## Public API

`MediaCenterItem`, `MediaCenterCategory`, `Categories`, `SelectedCategoryIndex`, `SelectedCategory`, `SelectedItem`, `IsDetailsOpen`, `State`, `SetCategories`, `SelectCategory`, `SelectItem`, `CloseDetails`, and `RequestPlayback`. `SelectionRequested` is used for both detail selection and playback intent; the host distinguishes the context.

## Behavior and visual tree

`PART_Grid` shows the active category. Selecting an item opens Details; Play raises the event and enters Playback; Close returns Browse. Empty categories are Empty, duplicate IDs are de-duplicated, and unknown IDs return false.
