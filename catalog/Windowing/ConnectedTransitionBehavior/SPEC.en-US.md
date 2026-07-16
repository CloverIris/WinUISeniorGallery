# ConnectedTransitionBehavior Specification

## Goal

Coordinate same-Window connected transitions between source/target while preserving focus/navigation transaction and deterministic fade for cross-window/missing elements.

## Host and Window Boundary

P0 candidate is same XamlRoot/Window only; move no real XAML and share no CompositionVisual across windows. Host supplies stable TransitionKey and navigation completion.

## Candidate Surface and Lock Conditions

Candidate concepts: TransitionKey, Role(Source/Target), Configuration, Prepare/Start/Cancel, TransitionCompleted. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Idle --> PreparedSource --> WaitingTarget --> Running --> Completed --> Idle\nAny --> Canceled/Fallback --> Idle
```

## Candidate Template Parts and Visual Tree

No fixed template; candidate layers are source snapshot/proxy Visual, target placeholder, and transition mask. Hide real target only when ready and always restore visibility.

## Behavior and Failure Modes

One transition per Key; navigation cancel, target timeout, Window change, or Reduced Motion uses instant/fade fallback. Every failure finally restores source/target.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

