# MediaPlayerChrome Acceptance

## Functional Scenarios

- Given a null session, when the control loads, then it is disabled and throws no exception.
- Given a Playing session, when Space is pressed, then Pause is called exactly once and rapid repeats do not run conflicting commands concurrently.
- Given an unhandled compact-overlay request, when the button is invoked, then one `PresentationRequested` is raised, no window is created, and state is unchanged.
- Given a host-confirmed full screen, when `IsFullScreen=true` is written, then icon, Automation name, and exit command update together.
- Given a replaced session, when the old session continues raising events, then the control ignores them.
- Given Stop or rate is unsupported, when presented, then its entry is hidden or disabled with an explanation.

## Timeline and State

- VOD, Live, and Live DVR sessions map to the correct timeline mode.
- Scrubbing does not continuously Seek; release commits exactly one final position.
- Buffering does not close a user-opened menu; Failed shows a localized non-blocking error and allows retry.

## Performance

- Playback position refreshes UI at no more than 10 updates per second.
- Idle auto-hide creates no continuous layout; display-mode switches leak no events or timers.
- After replacing a session 100 times, old sessions have zero remaining subscriptions.

## Input and Accessibility

- Verify core playback paths with mouse, touch, keyboard, touchpad, and gamepad.
- Light, Dark, High Contrast, Reduced Motion, and 100%–300% DPI remain usable.
- Every button target is at least 44×44 epx, focus order is stable, and screen readers announce actions rather than icon names.
- RTL mirrors layout while media-time progression follows the product localization contract.

## Automation

Unit-test state mapping, command serialization, session detach, and request events. UI-test missing-part degradation, responsive modes, keyboard paths, and AutomationPeer behavior. Use fake sessions with no network or real-media dependency.
