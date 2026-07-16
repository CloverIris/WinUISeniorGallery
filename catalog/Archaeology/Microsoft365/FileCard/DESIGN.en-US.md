# File Card Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Combine file identity, preview, metadata, and permission state in a compact surface; load progressively, keep actions permission-aware, and provide consistent unknown-type fallback.

## Modernize

Reconstruct as a Provider-independent FileCard: the host supplies metadata, preview stream, and commands; the card never reads disk, OneDrive, or SharePoint itself and performs no destructive action. Use current Fluent ThemeResources and system typography rather than pixel-copying historical UI.

## Responsive Behavior and Input

Card supports mouse, touch, keyboard, and screen reader; Open and More are separate, and destructive delete/move commands require host-owned confirmation. Narrow layouts collapse or layer content and wide layouts cap body width; 200% text scaling clips no essential content.

## Motion and Accessibility

Motion explains context, hierarchy, or loading only and Reduced Motion switches immediately. High Contrast never depends on shadow/transparency, and Automation exposes role, state, source, and next action.
