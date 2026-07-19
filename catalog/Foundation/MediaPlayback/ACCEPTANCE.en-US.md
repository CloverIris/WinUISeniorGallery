# MediaPlayback Acceptance

## Contract scenarios

- Given an empty or disposed session, when a command is invoked, then it returns `Cancelled/Disposed` without a cross-thread exception.
- Given an unsupported command, when a consumer requests it, then it returns `Rejected/Unsupported`.
- Given snapshot revision 4 followed by revision 3, when a consumer merges state, then revision 4 remains authoritative.
- Given a mutable buffered-range input, when the input changes after snapshot creation, then the snapshot remains unchanged.

## Automation and quality gate

Core unit tests cover value validation, range clamping, immutable copying, and all four command results. Media Adapter integration tests cover Dispatcher ordering, disposal, and late events. Core Release x64, architecture validation, and compilation by at least one Media consumer are required before review.

## Implementation Evidence

- 2026-07-16: Core Release x64 tests passed 4/4, Gallery Debug/Release x64 builds succeeded, and architecture tests passed 4/4.
- Real local-media adapter validation remains Gallery review work; the item remains `in-progress`.
