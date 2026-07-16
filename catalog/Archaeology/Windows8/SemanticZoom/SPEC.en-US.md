# Semantic Zoom Research Specification

The goal is to record navigation where zooming changes information meaning, not a control appearance. Historical tree: `Collection → DetailedView / OverviewGroups → GroupAnchor → DetailedView`; explanatory states: `Detail`, `TransitioningToOverview`, `Overview`, `Returning`, `NoGroupMapping`. Overview uses group/initial/time indexes and returns anchored to related detail.

Historical input included pinch, Ctrl+wheel, buttons, and keyboard; Gallery simulates only. With no group mapping, empty collection, disabled zoom, replaced data, or disabled motion, show a stable overview/explanation without losing accessible focus. Non-goals: zoom range, virtualization, `PART_*`, AutomationPeer, or real gesture API; the `SemanticZoomView` owner decides those.
