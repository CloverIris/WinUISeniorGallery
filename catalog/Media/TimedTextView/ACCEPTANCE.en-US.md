# TimedTextView Acceptance

## Functional Scenarios

- Given time at a Segment Start, then it is active; given time equal to End, then it is no longer active.
- Given overlapping Segments, then the latest Start is active, with stable input order breaking equal-Start ties.
- Given Karaoke with valid Words, then only current/elapsed word content is highlighted; without valid Words, highlight the whole Segment.
- Given Bilingual with same-Segment `TranslatedText`, then both original and translation appear; missing translation creates no blank second row.
- Given user invocation of a Segment, then one `SegmentInvoked` is raised and the control does not change `Position`.
- Given manual scrolling, then auto-centering pauses for five seconds and can be explicitly resumed.

## ASR Revisions

- A higher Revision for the same Document atomically replaces Interim text while stable-ID focus is retained.
- Equal or lower Revision is ignored; a changed Document ID may restart Revision.
- After a snapshot deletes a Segment, virtual items, indexes, and Automation tree remove the old ID.
- UI neither logs nor leaks recognition text and consumes no late update after unload.

## Large Documents and Performance

- 10,000 Segments use `ItemsRepeater` virtualization and do not create an equal number of XAML elements; only viewport plus buffers are realized by default.
- At 10 position updates per second, an unchanged active Segment causes no repeated event or list rebuild.
- After 100 full-document revisions, no old-snapshot event subscriptions remain; scroll-anchor drift stays within one active-item row height.

## Input, Theme, and Accessibility

- Verify mouse, touch, keyboard, and gamepad paths for SingleLine, ScrollingLyrics, Karaoke, and Bilingual.
- Light, Dark, High Contrast, Reduced Motion, RTL/mixed direction, 100%–300% DPI, and 200% text scaling have no clipping.
- Screen readers announce only active Segment changes, never every Word; virtualized ListItems expose position, time, language, and revision state.

## Automation

Unit-test normalization, time boundaries, overlap selection, Revision, track fallback, and translation fallback. UI-test virtualization, scroll suspension, focus anchoring, template degradation, and AutomationPeer. Use a virtual clock and immutable fake documents.

## Implementation Evidence

- 2026-07-16: Media Release x64 and Gallery Debug/Release x64 builds succeeded; Media automation tests passed 9/9.
- Gallery uses an explicitly labeled synthetic Document and virtual position control; it does not parse, upload, or persist caption content.
- The 10,000-item, assistive-tech, and theme/RTL/DPI matrices remain manual review work; the item remains `in-progress`.
