# Quick Resume Specification

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Historical Experience

Xbox Series X|S multi-game suspension and fast restoration; the system preserves several runtime states and returns users to the selected title from a group or recent list.

## Design DNA

Visible resumable state, recency ordering, state preview metadata, fast switching, and explicit non-resumable degradation rather than a generic launcher.

## Modern Reconstruction

Reconstruct as an in-app session-resume selector whose cards show preview, saved time, and resume capability. It restores only host-provided serialized state and never promises process or game-memory snapshots.

## Stable Implementation Boundary

This exhibit owns only historical research, modernization notes, and Gallery demo specification; stable implementation is owned by experiences.quick-resume (QuickResumeExperience). The exhibit declares no public types, resource keys, or platform services and never duplicates stable implementation. Gallery composes the modern dependency and labels the result Inspired by, never Original.

## States and Failure Modes

Available, Restoring, Unavailable, Expired, and Failed; restore is cancellable and failures keep the card with retry or restart. Missing dependency, data, or platform capability shows explanatory degradation and never fakes success. Closing the exhibit releases demo state and restores entry focus.
