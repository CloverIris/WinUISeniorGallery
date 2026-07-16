# Media Center Overlay Menu Research Specification

## Historical Prototype Structure

During playback, Windows Media Center opens a translucent remote-invoked overlay for transport, captions, audio, recording, or settings without leaving video. Over VideoSurface sit Scrim, OverlayPanel, PrimaryCommands, NestedSettings, and status/time; close fully restores video context.

## Historical Interaction States

```text
Hidden --> Opening --> Root\nRoot <--> Nested\nRoot/Nested --> Closing --> Hidden\nAny --> Disabled/Error
```

## Preserved Design DNA

Stay in playback context, short remote paths, translucent video awareness, hierarchical Back, and auto-hide paused by interaction.

## Explicitly Discarded and Modernized

Discard low contrast from transparency, focus loss from auto-hide, remote-only entry, and overlay-owned pause; use scrim, focus retention, multi-input, and host playback policy.

## Modern Owner and API Boundary

OverlayMenu owns hierarchy/focus; exhibit supplies Media Center command arrangement and history and owns no player. Dependency points only from Archaeology/Gallery to controls.overlay-menu; exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
ExhibitPage
|- Header (origin, era, proposed/pending)
|- PrototypeStructureDiagram (no original assets)
|- DesignDnaAndTradeoffs
|- ModernDemo (OverlayMenu)
|- InputAccessibilityMatrix
`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner/effect/data shows static structure and reason, never faking the product. Before specified, review sources, asset license, states, and modern differences.

