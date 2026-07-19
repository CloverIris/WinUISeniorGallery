# EditorCanvas Design

## Visuals and interaction

The canvas is the primary surface; the toolbar exposes Select/Pan/Rectangle/Text, Fit, Undo/Redo, Grid, and Snap. Objects are rounded cards with visible selection contrast. No Office screenshots or protected assets are copied, and preview drag state is separate from the committed Core revision.

## Responsiveness

Narrow windows wrap the toolbar and preserve a minimum canvas height; regular windows show the full toolbar; wide windows let the canvas expand. Wheel zoom anchors at the pointer and pan does not change world-coordinate orientation.

## Theme, motion, input, and accessibility

Use ThemeResources for Light/Dark and retain object/grid contrast in High Contrast. Reduced Motion has no inertia or transition. Arrow keys move one world unit, or GridSize when Snap is enabled. Automation names use `Canvas object {id}`.

## Modernization tradeoffs

Modernization keeps predictable document state and host ownership instead of embedding save, collaboration, file formats, or native pressure support. Archaeology owns provenance.

## Responsive input and accessibility
Mouse/touch/pen/keyboard, pressure, lasso with command equivalents; RTL affects UI not coordinates, High Contrast guides visible, Reduced Motion no inertia.
