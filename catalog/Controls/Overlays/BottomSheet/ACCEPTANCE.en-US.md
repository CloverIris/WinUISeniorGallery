# BottomSheet Acceptance

## Functional scenarios

- Given a 320 px window and Auto, when opened, then it enters from Bottom and stays inside safe bounds; given 1,000 px, it uses Side.
- Given 25%, 50%, and Content points, slow release selects nearest; velocity over 800 px/s selects one adjacent point without skipping.
- Given the minimum point, dragging 25% toward close or faster than 1,000 px/s closes with reason Drag.
- Given Modal, opening, Tab/Shift+Tab, Escape, and close keep focus inside then restore it, while UIA cannot operate background.
- Given Modeless, background remains operable and focus is not forcibly reclaimed.
- Given Close during Opening, animation reverses from the current frame and fires exactly one Closed and no Opened.
- Given host unload, forced HostUnloaded close releases capture, handlers, and Composition objects.

## Quality matrix

- Validate mouse drag, touch, touchpad, keyboard, Narrator; Light/Dark/High Contrast, RTL, Reduced Motion, and 100/200% DPI.
- Test soft keyboard, crossing 720 px, tiny windows, dynamic content growth, and runtime SnapPoints replacement.
- Pointer movement causes at most one layout/visual update per frame; Release x64 p95 frame time is at most 25 ms.
- Unit tests cover validation, cancelled Opening/Closing, event order, clamping, and velocity thresholds. UI tests cover focus trap, Z order, and topmost Escape behavior.
