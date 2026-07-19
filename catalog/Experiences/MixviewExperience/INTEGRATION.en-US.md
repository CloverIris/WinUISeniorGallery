# MixviewExperience Integration

## Contracts and resources

The control follows `foundation.motion-system` Reduced Motion semantics and adds no global resource keys. Nodes and content are host-provided; the control does not access files, network, or media services.

## Host boundary

The host owns navigation, recommendations, persistence, and content lifetime. The control only reports through `NodeSelected` and `Closed`; it creates no windows or background singletons.

## Lifecycle and threading

Call `SetNodes`, `Open`, `Close`, and `SelectNode` on the UI thread. After unload, the host should stop external updates to the instance.

## Failure and privacy

Invalid nodes report `ArgumentException`; unknown selection returns false. No capabilities, telemetry, uploads, or persistence are used.
