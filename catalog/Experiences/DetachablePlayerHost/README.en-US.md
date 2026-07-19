# DetachablePlayerHost

The Chinese document is normative. This `in-progress` lab implements Inline/Detached lifecycle coordination and a local fake-host demo.

## Status

in-progress / lab / P1

## Scope

The control emits floating-host requests and owns serialization, cancellation, stale-response filtering, and owner-close fallback. It never creates a window, migrates a MediaPlayer, or owns a playback session.

## Ownership

Implementation: `src/WinUI3.Senior.Windowing/DetachablePlayerHost.cs`; demo: `src/WinUI3.Senior.Gallery/Pages/DetachablePlayerHostPage.xaml*`.
