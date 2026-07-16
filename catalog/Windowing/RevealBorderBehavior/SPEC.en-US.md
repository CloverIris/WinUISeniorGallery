# RevealBorderBehavior Specification

## Goal

Provide pointer-proximity border illumination for in-app interactive elements with safe modern-theme/accessibility degradation.

## Host and Window Boundary

Attach only to XAML elements in the current Window; create no light window, track no global pointer, and change no hit testing/command.

## Candidate Surface and Lock Conditions

Candidate concepts: IsEnabled, Radius, Intensity, BorderBrush, PressedIntensity, UseSharedLight. These are not public API; freeze types, defaults, event cancellation/threading, and failure results before ready.

## State Diagram

```text
Detached --> Idle --> PointerNear --> PointerOver --> Pressed\nAny --> Disabled/HighContrast/Unloaded
```

## Candidate Template Parts and Visual Tree

No template parts; candidate Composition tree is per-window shared light plus per-element border mask. High Contrast/Reduced Motion may disable light while retaining standard border.

## Behavior and Failure Modes

Compute only visible/enabled/hit-test elements; clear on Window deactivation or PointerExited. Shared light registers weakly and unload unregisters immediately.

## Ready Promotion Gate

Freeze API, full state table, part/nonvisual contract, Window Closed/cancel/rollback, resources, platform-version fallback, performance, and Automation with synchronized bilingual API/IDs before ready.

