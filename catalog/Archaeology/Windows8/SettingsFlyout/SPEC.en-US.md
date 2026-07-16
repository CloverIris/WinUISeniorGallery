# Settings Flyout Research Specification

Historical tree: `AppContent → SettingsEntry → Flyout → Header/Back/SettingsList → Detail`. Explanatory states: `Closed`, `Root`, `Detail`, `Dirty`, `Saving`, `SaveFailed`, `Unavailable`. It records edge settings hierarchy and return path, not settings storage, validation, commit, window sizing, `PART_*`, or public events. Gallery simulates states only: close/back restores trigger focus; save failure is explained; missing host, permission, or data source never creates fake success.
