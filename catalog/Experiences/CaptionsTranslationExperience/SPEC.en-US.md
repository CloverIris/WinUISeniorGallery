# CaptionsTranslationExperience Specification

## Goal

Provide a host revision layer for `TimedTextView` with monotonic ordering, translation-track fallback, and degraded-error state.

## Non-goals

No speech recognition, translation, caption-file parsing, network access, or provider instantiation.

## Public API

`CaptionsTranslationRevision(Revision, Source, Translation, IsFinal, ErrorCode)`; `State` (Idle/Listening/Translating/Degraded/Error), `DisplayMode`, `IsAutoFollowEnabled`, `ApplyRevision`, `SetPosition`, and `SetError`. Revisions less than or equal to the current value are rejected without changing the projection.

## Projection

Required parts are `PART_TimedTextView` and `PART_Status`. Source and Translation tracks are merged into an immutable document; duplicate track IDs keep the first. Error revisions expose degraded state while retaining the last accepted document. Provider and unload lifetimes remain host-owned.
