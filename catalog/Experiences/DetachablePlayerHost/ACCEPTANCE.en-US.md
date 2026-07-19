# DetachablePlayerHost Acceptance

## Current gate

The item is `in-progress`; the fake host supports manual observation, while visual and runtime verification remain before `review`.

## Given / When / Then

- Given a bound host in Inline, When `DetachAsync` is called, Then exactly one Open request is sent and state becomes Detached.
- Given Detached, When `AttachAsync` is called, Then exactly one Close request is sent and state returns Inline.
- Given an operation is pending, When the owner closes or the host is replaced, Then the late result is cancelled and cannot overwrite new state.
- Given no host, an invalid size, or an invalid state, When an operation is called, Then a stable Rejected error code is returned.

## Matrix

Light/Dark/High Contrast, Reduced Motion, RTL, 100%/150%/200% DPI, keyboard buttons, window disposal, and cancellation.
