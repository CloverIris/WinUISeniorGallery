# Zune Now Playing Research Specification

## Historical Prototype Structure

Zune software full-screen Now Playing blends artist imagery, album art, ambient color, and large typography into the background, revealing subdued controls on demand. AmbientBackground, Artist/AlbumVisual, Metadata, Progress/Transport overlay, and optional queue/lyrics; content and control layers are separate.

## Historical Interaction States

```text
LoadingArt --> AmbientReady\nAmbientReady <--> ControlsVisible\nPlaying <--> Paused/Buffering\nAny --> MissingArtFallback/Error
```

## Preserved Design DNA

Immersive content-first hierarchy, media-derived color, progressively disclosed controls, and title/image storytelling.

## Explicitly Discarded and Modernized

Discard default network artist downloads, low-contrast text on art, focus loss from auto-hide, and continuous background motion; use licensed Provider, scrim, focus retention, Reduced Motion.

## Modern Owner and API Boundary

ImmersiveNowPlaying composes stable MediaPlayerChrome/TimedText; exhibit supplies Zune-inspired theme and research only. Dependency points only from Archaeology/Gallery to experiences.immersive-now-playing; the exhibit declares no type, resource key, service, or platform capability.

## Gallery Exhibit Tree

```text
+ExhibitPage
+|- Header (era, product, status)
+|- PrototypeStructureDiagram (original assets forbidden)
+|- DesignDnaAndTradeoffs
+|- ModernDemo (ImmersiveNowPlaying)
+|- InputAccessibilityMatrix
+`- SourcesAndCopyright
```

## Failure and Lock Conditions

Missing owner, demo data, or effect shows a static diagram and reason and never fakes the original product. Before specified: recheck sources, asset license, prototype states, and modern differences.

