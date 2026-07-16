# Editor Canvas Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Separation of content surface and tool chrome, stable coordinate space, continuous zoom, alignment guides, page/slide navigation, and selection-driven contextual tools.

## Modernize

Reconstruct as a generic editor-canvas shell composing zoom/pan, page thumbnails, rulers, and guides; document model, graphics rendering, undo, and collaboration belong to the host or future Canvas module. Use current Fluent ThemeResources and system typography rather than pixel-copying historical UI.

## Responsive Behavior and Input

Mouse/pen/touch gestures coexist with keyboard shortcuts; Space temporarily pans, Ctrl+wheel zooms, and screen readers navigate a structured object tree rather than pixels. Narrow layouts collapse or layer content and wide layouts cap body width; 200% text scaling clips no essential content.

## Motion and Accessibility

Motion explains context, hierarchy, or loading only and Reduced Motion switches immediately. High Contrast never depends on shadow/transparency, and Automation exposes role, state, source, and next action.
