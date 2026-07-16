# Focus Sessions Research Specification

## Historical Prototype Structure

Windows 11 Clock Focus Sessions combines focus/break timer, daily progress, Microsoft To Do tasks, and optional Spotify music into a startable focus flow. DurationPicker, focus/break cycles, TimerRing, TaskPicker, DailyProgress, AudioProvider, and Start/Pause/End controls form the session panel.

## Historical States and Focus

```text
Idle --> Preparing --> Focusing <--> Paused\nFocusing --> Break --> Focusing\nFocusing/Break --> Completing --> Completed\nAnyActive --> Ending --> Idle\nAny --> ProviderPartial/Error
```

Space/A starts/pauses, End confirms, and keyboard/touch/gamepad work; announce minute/stage changes, never every second to Narrator.

## Preserved Design DNA

Time tied to task, focus/break rhythm, visible progress, optional music that never blocks timer, and end summary.

## Modernization and Discarded Boundary

Discard mandatory Microsoft account, hard Spotify/To Do dependency, automatic system Do Not Disturb control, and cloud tracking; core timer works locally with explicit Provider consent.

## Modern Feature Owner

Owner registry: `experiences.focus-session` (`FocusSession`).

FocusSession owns session state and Provider abstractions; exhibit uses virtual clock/fake tasks and connects to no Microsoft service. Archaeology dependencies point only Gallery/Archaeology to owner; stable modules never reference exhibits.

## Gallery Research Tree

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (FocusSession)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## Failure and Promotion Locks

Missing owner/capability/data retains static research and never fakes system behavior. Before specified, lock sources, owner, asset license, differences, Automation semantics, and platform exclusion zone.
