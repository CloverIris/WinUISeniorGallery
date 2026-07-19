# BigTitle Specification

## Goal

Provide a scroll-driven Metro/Fluent large-title collapse control. It synchronizes title visuals only and owns neither navigation nor ScrollViewer lifetime.

## Non-goals

No navigation, routing, persistence, automatic ScrollViewer creation, or page reconstruction.

## Public API

`Text`, `ExpandedFontSize`, `CollapsedFontSize`, `CollapseDistance`, read-only `CollapseProgress`, `IsSticky`, and `ScrollSource`; read-only `State` is Expanded/Collapsing/Collapsed. A null ScrollSource returns progress to 0.

## State model

VerticalOffset/CollapseDistance is clamped to 0..1; text/font changes refresh immediately; unload detaches ViewChanged so a departed page receives no callbacks.

## Template parts and visual tree

`PART_TitlePresenter` and `PART_CompactPresenter` are optional; missing parts only disable their layer and do not stop state calculation. Font size interpolates linearly, with Expanded opacity decreasing and Compact opacity increasing.

## Behavior and failure modes

Unavailable ScrollSource keeps the title Expanded. Reduced Motion does not alter state or task completion; the template simply avoids extra animation.

## Open Decisions

Open Decisions: minimum height under 200% text scaling, host layout constraints for Sticky visuals, and RTL title alignment.

## Proposed implementation baseline
API: `Text`, `ExpandedFontSize=64`, `CollapsedFontSize=20`, read-only `CollapseProgress`, `ScrollSource`, `IsSticky=true`. States: `Expanded/Collapsing/Collapsed`; parts: `PART_TitlePresenter`, `PART_CompactPresenter`. Clamp progress 0..1; missing source returns Expanded. Lock minimum height after a no-clipping 200% text prototype.
