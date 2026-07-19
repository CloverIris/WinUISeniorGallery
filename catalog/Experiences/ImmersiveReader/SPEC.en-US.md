# ImmersiveReader Specification

## Goal

Use virtualized reader blocks for immersive reading, focus anchoring, line focus and host-neutral read-aloud requests.

## Non-goals

It does not parse files, mutate source text, create a SpeechSynthesizer, or persist reading position.

## Public API

`ReaderBlock(Id, Text, IsHeading, HeadingLevel, Tag)`; `ImmersiveReader.SetBlocks`, `FocusBlock`, `MoveFocus`, `ToggleReading`; `FocusMode`, `FontScale`, `IsLineFocusEnabled`; and `BlockInvoked`, `FocusChanged`, `SpeechRequested` (handled by the host).

## State model

`Empty`, `Ready`, and `Idle/Reading`; focus changes move the view and announce the current block. Reading is a request state and does not claim that platform speech executed.

## Template parts and visual tree

Required `PART_ItemsRepeater` and `PART_LiveRegion`. ItemsRepeater provides the realization window for large documents; data and focus APIs remain usable without the template.

## Behavior and failure modes

Duplicate ids keep the first block; empty text is excluded. Up/Down moves focus, Enter/Space announces user invocation, and R emits a speech request. Font scale is constrained to 0.8–2.5; line focus changes visuals only.

## Open Decisions

Remaining review items are sentence/word models, a real speech provider, document parsing and privacy. The current implementation is a block-level P2 laboratory.

## Scenario, data, and visual tree
Document→Block→Sentence→Word; tree `Toolbar→DocumentPresenter→FocusMask`; Loading/Reading/Speaking/Paused/Error; display settings never mutate source.
