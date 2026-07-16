# MediaCenterGrid Specification

## Goal

Define a virtualized poster grid for TV, remote, and living-room distance, enlarging focus and showing details during 2D navigation.

## Host and Window Boundary

Remain in the current page visual tree; create no window, full screen, or playback. Host supplies ItemsSource, templates, navigation, and details; external services own windows, playback, and loading.

## Non-goals

No media indexing, recommendation, image cache, playback queue, or DRM.

## Candidate Surface and Lock Conditions

Candidate concepts are ItemsSource, ItemTemplate, SelectedItem, FocusedItem, Columns, FocusScale, DetailsTemplate, and ItemInvoked; not committed API. Freeze selection/focus, async items, alignment, and Automation before ready.

## State Diagram

```text
Empty --> Loading --> Ready\nReady --> FocusMoving --> Ready\nReady --> DetailsVisible --> Ready\nLoading/Ready --> PartialError\nAny --> Unloaded
```

## Candidate Template Parts and Visual Tree

Candidate visual tree: ScrollViewer → ItemsRepeater → virtualized posters plus DetailsPresenter, FocusChrome, and Loading/Empty/Error presenters; scale uses render transform only.

## Behavior and Failure Modes

Directional input follows geometric neighbors and never jumps due to async images; details load after focus reaches a safe zone. Refresh preserves stable ID or nearest visible neighbor.

## Ready Promotion Gate

Freeze API/defaults/threading, template contract, transition table, destruction/cancellation, resources, performance, and AutomationPeer with synchronized bilingual API/IDs before ready.

