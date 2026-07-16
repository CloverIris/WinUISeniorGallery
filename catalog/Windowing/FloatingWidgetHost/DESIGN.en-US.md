# FloatingWidgetHost Design

## Visual Hierarchy and Responsive Behavior

Per-window candidate tree: WindowChrome, WidgetTitleBar, ContentPresenter, Loading/Error presenters; owner retains only a lightweight handle/state. Narrow/regular/wide and 100%–300% DPI share one state model; Window change never rebuilds host data and 200% text uses reflow/overflow.

## Input and Focus

Widget has independent focus scope, Alt+F4/system menu/keyboard close; topmost never blocks app switching, and focus restores only if owner survives.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not depend on material/transparency. Reduced Motion uses instant/fade, targets are 44×44 epx, and Automation exposes window state, action, and error.

## Modernization Tradeoffs

Archaeology owns provenance; never pixel-copy system Shell and preserve system menu, Snap, Caption Buttons, and reserved gestures.

