# FloatingWidgetHost Acceptance

## Current Gate

Remain proposed with no .cs/.xaml; ready only after SPEC gates and bilingual synchronization.

## Given / When / Then

Given owner policy Close, when owner closes, then widgets cancel work before close. Given ContentFactory throws, then no ghost window/handle and retry remains possible.

## Input, Focus, and Cross-window Lifecycle

Widget has independent focus scope, Alt+F4/system menu/keyboard close; topmost never blocks app switching, and focus restores only if owner survives. Cover Closed, unload, cancel, DPI/display/theme change, and host rejection; completion leaves no focus trap, late event, or Window reference leak.

## Performance Budget

Empty widget creation target under 500 ms and move/resize at 60 Hz; after closing 100 windows, AppWindow, Dispatcher callbacks, and content refs reach zero.

## Automation Matrix

Architecture tests ID/dependency/headings; unit tests state/cancel/rollback; UI tests Light/Dark/High Contrast, Reduced Motion, mouse/keyboard/touch/pen/gamepad, Narrator, RTL, 100%–300% DPI, 200% text, and multiwindow close ordering.

## Ready Promotion Acceptance

Deterministic fake Window/AppWindow, virtual clock, and failure injection pass with repeatable platform fallback/Automation/performance; otherwise remain proposed.

