# MixviewExperience Design

## Information hierarchy

The center node is the current context; related nodes orbit in `RelatedIds` order. Titles remain readable and focus visuals remain visible. The Gallery uses synthetic text only.

## Responsive layout

The center is calculated from available width and height. Narrow windows preserve the center and may reduce density through `MaxVisibleNodes`. Node buttons target a 48×48 minimum touch area.

## Motion and input

The current implementation uses deterministic redraw; `IsReducedMotion` is reserved for future transitions. Mouse, touch, and keyboard use Button/focus semantics, while Escape closes.

## Accessibility and themes

Each node has an Automation Name and `PART_LiveRegion` politely announces user selection. Light, Dark, and High Contrast use theme resources; RTL does not reverse relationship order or IDs.

## Modernization tradeoffs

The “related content expands around the current content” DNA is retained while proprietary recommendation services are replaced with a host-owned node model.
