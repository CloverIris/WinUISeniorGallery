# DockLayoutPreview Design

## Visual Hierarchy and Responsive Behavior

Candidate tree: OverlayRoot, TargetRepeater, PreviewRectangle, InvalidDropPresenter; overlay hit testing only during drag. Narrow/regular/wide and 100%–300% DPI share one state model; Window change never rebuilds host data and 200% text uses reflow/overflow.

## Input and Focus

Mouse/touch/pen drag equals keyboard Dock commands; screen readers enumerate and confirm targets without precise pointing.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not depend on material/transparency. Reduced Motion uses instant/fade, targets are 44×44 epx, and Automation exposes window state, action, and error.

## Modernization Tradeoffs

Archaeology owns provenance; never pixel-copy system Shell and preserve system menu, Snap, Caption Buttons, and reserved gestures.

