# Captions.Abstractions Specification

## Goal

Define reusable responsibilities, state, and boundaries.

## Non-goals

No implementation while proposed.

## Public API

Not locked.

## State model

Not locked.

## Template parts and visual tree

Not locked.

## Behavior and failure modes

Follow referenced contracts.

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Candidate model (not public API)
Candidate `CaptionSession/Track/SegmentRevision/WordTiming`; Idle/Starting/Streaming/Completing/Failed; monotonic RevisionId, stable SegmentId, unified TimeSpan. No network/credential API.
