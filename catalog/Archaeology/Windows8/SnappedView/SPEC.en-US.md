# Snapped View Research Specification

## Historical model

Windows 8 divided an app into primary and narrow snap regions; changing width changed visible information and app state. The research tree is `Workspace → LayoutHint → PrimaryRegion + SnapRegion → RegionContent`; explanatory states are `Single`, `Previewing`, `Snapped`, `ReflowRequired`, and `Unavailable`. This is historical description, not a contract for window sizing or docking algorithms.

## Input and failure boundary

Historical entry used edge drag, touch drag, and system gestures; Gallery offers buttons, keyboard, and simulated drag only. When a region is too narrow, do not shrink text to illegibility: annotate reflow or show a stacked diagram. With no multi-window capability, cancelled drag, host rejection, or disabled motion, return to static layout and never create/move windows.

## Non-goals

Do not hijack system Snap, guarantee 1/3 or 1/2 ratios, persist layouts, or define `PART_*`/public commands. Modern `DockLayoutPreview` owns real preview, window boundaries, and failure semantics.
