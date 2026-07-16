# Immersive Reader Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

The immersive reading experience grew from OneNote Learning Tools and later appeared across Word, OneNote, Edge, and Azure: isolated text, read-aloud highlighting, line focus, grammar/syllables, and translation support comprehension.

## Design DNA

Distraction removal, progressively disclosed reading aids, user-controlled typography, synchronized visual/audio reading, and support for dyslexia and language learning.

## Modern Reconstruction

Reconstruct as a local-first focused-reading shell accepting structured text with width, size, spacing, line focus, and read position; speech, translation, dictionary, and picture services arrive through separate Providers.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by experiences.immersive-reader (ImmersiveReaderExperience). The exhibit declares no public API and duplicates no modern component. Historical names are research titles only and the demo is labelled Modern Reconstruction.

## States and Failure Modes

Reading, Paused, LineFocus, PreferencesOpen, and ServiceUnavailable; cloud failures never remove local text or user typography settings. Missing data, service, or dependency uses local, explanatory degradation and never presents fake data as a real account result.
