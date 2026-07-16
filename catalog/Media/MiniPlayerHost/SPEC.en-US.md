# MiniPlayerHost Specification

## Goal

Define an in-window host that collapses a full player into an edge mini player while preserving one playback session, focus return, and content state.

## Host and Window Boundary

Manage two slots inside the current Window; create no AppWindow, request no CompactOverlay, and never reparent XAML across windows. CompactOverlayHost/DetachablePlayerHost owns windowing and MediaPlayerChrome/IMediaPlaybackSession owns playback.

## Non-goals

No decoding, playback-session lifetime, automatic dock choice, or preference persistence.

## Candidate Surface and Lock Conditions

Candidate concepts are Content, MiniContent, Mode, PreferredDock, ExpandRequested, CollapseRequested, and TransitionFailed; not public API. Freeze types, cancellation, default dock, and template contract before ready.

## State Diagram

```text
Expanded --> Collapsing --> Mini\nMini --> Expanding --> Expanded\nTransition --> Failed --> previous stable state\nAny --> Unloaded --> host content restored
```

## Candidate Template Parts and Visual Tree

Candidate visual tree: RootGrid contains ExpandedPresenter, MiniPresenter, TransitionLayer, and FocusSentinel; only one Presenter owns interactive content and the other is a lightweight placeholder.

## Behavior and Failure Modes

Host may reject requests; duplicates coalesce and opposite requests reverse from current progress. Close, unload, or session replacement cancels without disposing host objects.

## Ready Promotion Gate

Freeze API/defaults/threading, template contract, transition table, destruction/cancellation, resources, performance, and AutomationPeer with synchronized bilingual API/IDs before ready.

