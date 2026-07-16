# Charms Bar Design Archaeology and Modernization

## Retained design DNA

Retain progressive disclosure—edge hint, temporary command layer, contextual second pane—semantic grouping, and staying in the current task. A modern exhibit has an explicit in-app boundary, title, and close action; commands cannot depend on icon-only recognition.

## Rejected legacy

Reject undiscoverable hot corners, cross-app overlays, fixed five system semantics, forced full screen, and hiding important work at an edge. A modern `EdgeCommandPanel` must be opened by an explicit host request and obey application navigation and permission boundaries.

## Responsive, accessibility, and motion

Narrow windows show a labelled bottom/side alternative; regular and wide windows may use an end-side panel. At 150%/200% DPI, text scaling, Chinese long labels, and RTL, use logical end rather than physical right. Light/Dark/High Contrast use theme resources; Reduced Motion switches visibility directly. Tab order is trigger, rail, pane, close; Esc/Back closes one layer; Narrator announces the layer and unavailable reasons.
