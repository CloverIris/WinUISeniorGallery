# People Card Design

## Additional Given / When / Then
- Given historic/modern side by side, state/input changes make structure, focus, and removals traceable.
- Given narrow, RTL, High Contrast, or Reduced Motion, the task remains possible and Narrator gets name/role/state/position.
- Given unapproved sources/license, show original placeholders only; add no API and do not enter review/stable.

## Preserve

Identity context without navigation, progressive disclosure, unified identity/presence, actions adjacent to the person, and section-level degradation under limited permission.

## Modernize

Reconstruct as a data-source-independent PeopleCard: the host supplies a minimal person model and on-demand section Providers; local data displays by default and the card never calls Microsoft Graph itself. Use current Fluent ThemeResources and system typography rather than pixel-copying historical UI.

## Responsive Behavior and Input

Hover previews only, keyboard focus or click can pin, Escape closes and restores the trigger; touch has no hover dependency and presence/avatar have textual alternatives. Narrow layouts collapse or layer content and wide layouts cap body width; 200% text scaling clips no essential content.

## Motion and Accessibility

Motion explains context, hierarchy, or loading only and Reduced Motion switches immediately. High Contrast never depends on shadow/transparency, and Automation exposes role, state, source, and next action.
