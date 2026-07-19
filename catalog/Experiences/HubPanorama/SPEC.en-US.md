# HubPanorama Specification

## Goal

Provide Metro Hub-style horizontal section exploration. The control owns section selection, scroll alignment, and directional-key semantics; the host owns content, backgrounds, and routing.

## Non-goals

No network assets, window creation, route persistence, or duplicated Pivot API.

## Public API

`HubSection` (Id/Header/Items/Background), `Sections`, `SelectedIndex`, `SectionWidth`, `IsParallaxEnabled`, `ParallaxStrength`, `IsWrapNavigationEnabled`, and `IsReducedMotion`; methods `SetSections`, `SelectSection`, and `Navigate`; event `SectionChanged`.

## State model

An empty collection has SelectedIndex=-1. Sections are horizontally arranged; Left/Right mirror under RTL, Home/End jump to boundaries, and wrapping cycles or clamps as configured. Scroll offset repairs the current section.

## Template parts and visual tree

`PART_ScrollViewer` and `PART_Repeater` are required; `PART_Indicator` is optional. Missing Indicator does not affect selection or scroll, and backgrounds are not required to have a specific type.

## Behavior and failure modes

Duplicate section IDs retain the first item; SectionWidth is clamped to 240–2000. Reduced Motion disables ChangeView animation without changing selection. Hosts detach ScrollViewer events on unload.

## Open Decisions

Open Decisions: narrow-window vertical fallback, actual background-parallax pipeline, and virtualization budget for 10k sections.

## Scenario, data, and visual tree
Section(Id,Header,Template,Items,BackgroundLayer); tree `HubRoot→SectionRepeater`; Empty/Ready/Panning/Settling; current section is route bookmark, not duplicate Pivot API.
