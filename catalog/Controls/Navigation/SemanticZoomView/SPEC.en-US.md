# SemanticZoomView Specification

## Goal

Switch between a detailed list and a grouped overview. The host supplies group-key and title selectors; the control owns only group projection, mode, focused group, and input semantics.

## Public API

`ItemsSource`, `Mode=Detail`, `IsZoomEnabled=true`, `FocusedGroupIndex`, `IsReducedMotion`, `GroupKeySelector`, `GroupTitleSelector`, and `Groups`; methods `RebuildGroups`, `ZoomOut`, `ZoomIn`, and `InvokeGroup`. `ZoomChanged` is raised after a committed mode change and `GroupInvoked` when an overview item is invoked.

## Grouping and state

Null/blank keys normalize to `#`; groups sort case-insensitively using current culture, and an empty list keeps `FocusedGroupIndex=-1`. Overview supports keyboard/wheel group selection, Enter/Plus zoom-in, and Escape/Minus zoom-out. Left/right group navigation mirrors in RTL.

## Template and degradation

`PART_ZoomedInView` and `PART_ZoomedOutView` are required and missing parts throw a clear template exception; `PART_Viewport` is optional. Reduced Motion disables transition animation. The control does not duplicate the source and binds only `_groups` to the overview.

## Current boundary

The item is `in-progress/lab/P1`; pinch gestures, cross-group targeting, and persisted zoom state require separate review.
