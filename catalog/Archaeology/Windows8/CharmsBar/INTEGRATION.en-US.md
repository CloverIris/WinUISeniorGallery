# Charms Bar Research Exhibit Integration

## Ownership and host boundary

Modern API, resource keys, template parts, and window lifecycle are owned by `windowing.edge-command-panel` and Windowing Contracts. This exhibit may read their public documentation and show static explanation or local demo state in Gallery; it must not call system edge APIs, register shortcuts, create windows, or request capabilities.

## Lifecycle, threading, and diagnostics

Demo data is local read-only JSON/Markdown. On unload, cancel explanatory motion and focus-restoration requests. UI state changes on the UI thread only; no user content, network, or telemetry is used. Missing host, unimplemented panel, unsupported backdrop, or rejected request produces local Gallery diagnostics and fallback copy.

## Resources and privacy

Use Gallery theme resources and self-made placeholders; do not use Windows icons, screenshots, or extracted assets. Future external links must disclose leaving Gallery. This exhibit has no permissions, accounts, media, or personal-data boundary.
