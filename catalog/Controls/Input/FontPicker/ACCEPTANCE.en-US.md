# FontPicker Acceptance

## Current gate

The item is `in-progress / lab / P2` and may enter `review` only after template, input, and accessibility checks are complete.

## Given/When/Then

- Given 1000 options with duplicate family names, When `SetOptions` runs, Then options are de-duplicated and search order is stable.
- Given `IsFavoritesOnly=true`, When an option is favorited or unfavorited, Then `FavoritesChanged`, `OptionsChanged`, and `VisibleOptions` update together.
- Given the selected option is removed by a replacement list, When `SetOptions` completes, Then `SelectedFontFamily` is cleared without an exception.
- Given an unknown family is assigned externally, When the property changes, Then the string remains available to the host but no selection event is raised.

## Experience matrix

Light, Dark, High Contrast, 100%/150%/200% DPI; keyboard, mouse, touch, Narrator, and Reduced Motion; Chinese, English, and RTL. Search, list, and favorite controls have visible focus and stable Automation names.

## Performance and failure

Rebuilding a 1000-option view should complete within one Dispatcher batch; an empty list shows Empty; unknown selection returns `false`; font availability is host-labelled and does not read the registry.
