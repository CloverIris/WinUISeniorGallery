# IconPicker

`IconPicker` is a safe selector over explicitly registered application icon catalogs. It provides search, categories, favorites, recents, and committed selections without scanning DLL/EXE files, system resources, or font files, and without persisting user preferences.

## Status

in-progress / lab / P2.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Controls/Input/IconPicker

Implementation: `src/WinUI3.Senior.Controls/Input/IconPicker`

## Implementation readiness
The current implementation includes `IconPicker`, `IconPickerSource`, and `IconPickerItem`. Sources are host-injected, search uses a 200 ms debounce by default, unavailable or glyph-less entries cannot be committed, and a bounded in-memory recent list is maintained. Directional metadata is explicit and glyphs are not silently mirrored in RTL. Gallery integration and automated verification remain follow-up work.
