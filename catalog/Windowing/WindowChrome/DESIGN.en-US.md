# WindowChrome Design

## Visual Hierarchy and Responsive Behavior

Candidate tree: ChromeRoot, TitleBarHost, ContentPresenter, BackdropLayer; system draws CaptionButtons by default and names freeze before ready. Narrow/regular/wide and 100%–300% DPI share one state model; Window change never rebuilds host data and 200% text uses reflow/overflow.

## Input and Focus

Drag regions contain only noninteractive background; Alt+Space, double-click maximize, Snap Layout, keyboard, and touch retain system semantics.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not depend on material/transparency. Reduced Motion uses instant/fade, targets are 44×44 epx, and Automation exposes window state, action, and error.

## Modernization Tradeoffs

Archaeology owns provenance; never pixel-copy system Shell and preserve system menu, Snap, Caption Buttons, and reserved gestures.

