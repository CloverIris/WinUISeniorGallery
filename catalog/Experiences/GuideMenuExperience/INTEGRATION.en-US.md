# GuideMenuExperience Integration

## Dependencies

windowing.edge-command-panel

## Global contracts and resources

Do not redefine global contracts or shared resource keys.

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## Ownership, boundaries, and lifecycle
Composes EdgeCommandPanel; host supplies commands/navigation. No system Guide interception; unload cancels command/restores focus.
