# CaptionsTranslationExperience Acceptance

## Current gate

The item is `in-progress`; the Gallery uses synthetic text and intentionally contains no provider, ASR, or translation implementation.

## Given / When / Then

- Given current Revision=2, When Revision=1 is applied, Then it returns false and document/state do not roll back.
- Given Source and Translation tracks, When a newer revision is applied, Then TimedTextView can enter Bilingual with both tracks.
- Given an ErrorCode, When a revision is applied, Then state is Degraded and the last accepted text remains visible.
- Given missing template parts, When the control runs, Then pure revision logic still applies and only visual projection degrades.
