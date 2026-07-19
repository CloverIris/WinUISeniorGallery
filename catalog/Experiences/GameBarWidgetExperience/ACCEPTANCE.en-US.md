# GameBarWidgetExperience Acceptance

## Current gate

The item is `in-progress`; the local fake host is observable, while visual and runtime verification remain.

## Given / When / Then

- Given a bound host, When Open is called, Then one host request is sent and state becomes Interactive.
- Given Interactive, When the host confirms ClickThrough, Then state becomes ClickThrough and the recovery hotkey remains available.
- Given no acknowledgement or an empty recovery hotkey, When ClickThrough is requested, Then it returns false and preserves the prior mode.
- Given ClickThrough, When Minimize/Restore is used, Then content is retained and ClickThrough is restored.
