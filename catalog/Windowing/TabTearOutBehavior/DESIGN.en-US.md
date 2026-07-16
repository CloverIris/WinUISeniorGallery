# TabTearOutBehavior Design

## Visual Hierarchy and Responsive Behavior

Candidate visuals contain DragAdorner, DropTargets, and TransferPlaceholder; real content is newly created by ContentFactory on target DispatcherQueue. Narrow/regular/wide and 100%–300% DPI share one state model; Window change never rebuilds host data and 200% text uses reflow/overflow.

## Input and Focus

Pointer tear-out, keyboard Move to new window, and screen-reader command are equivalent; cancel restores source, target close reattaches or closes document by host policy.

## Theme, Motion, and Accessibility

Use ThemeResource only; High Contrast does not depend on material/transparency. Reduced Motion uses instant/fade, targets are 44×44 epx, and Automation exposes window state, action, and error.

## Modernization Tradeoffs

Archaeology owns provenance; never pixel-copy system Shell and preserve system menu, Snap, Caption Buttons, and reserved gestures.

