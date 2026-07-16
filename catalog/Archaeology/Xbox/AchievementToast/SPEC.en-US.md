# Achievement Toast Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

The achievement-unlock notification pattern established by Xbox 360: a brief audiovisual cue, icon, title, and score appears over gameplay, then exits without taking input focus.

## Design DNA

High-recognition feedback for rare events, short non-modal presentation, strict queuing, one highlighted achievement at a time, and cumulative progress available in details.

## Modern Reconstruction

Reconstruct as a general achievement toast: the host supplies localized title, icon, progress, and rarity; support queueing, coalescing, timeout, and motion-free mode without copying Xbox audio or graphics.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by controls.achievement-toast (AchievementToast). The exhibit declares no public types, resource keys, or platform services and never duplicates stable implementation. Gallery composes the modern dependency and labels the result Inspired by, never Original.

## States and Failure Modes

Queued, Entering, Visible, Exiting, and Dismissed; a repeated ID updates only an item not yet shown and never replays a visible item. Missing dependency, data, or platform capability shows explanatory degradation and never fakes success. Closing the exhibit releases demo state and restores entry focus.
