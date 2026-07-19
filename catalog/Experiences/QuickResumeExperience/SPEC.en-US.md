# QuickResumeExperience Specification

## Goal

Provide a ten-foot recent-session picker ordered by last activity. It emits restore/remove requests without owning a player or persistence.

## Non-goals

It does not read files, create thumbnails, persist resume tokens, create windows, or execute restoration.

## Public API

`QuickResumeEntry(Id, Title, Position, Duration, LastActive, Thumbnail, Tag)`; `QuickResumePicker.SetEntries` deduplicates and orders entries; `InvokeSelected`, `RemoveSelected`, `MoveFocus`; `ItemInvoked` and cancellable `RemoveRequested`.

## State model

`Empty` and `Ready`; Resuming and Removing are host request states. Completed or non-resumable entries do not raise `ItemInvoked`.

## Template parts and visual tree

The default template uses an `ItemsWrapGrid` and progress cards; ListView provides realization. The API remains consumable if the visual template is missing.

## Behavior and failure modes

Duplicate ids keep the most recently active entry and the collection is capped by `MaximumItems` (12 by default). Direction keys/gamepad move focus, Enter/Space/GamepadA requests resume, and Delete/GamepadX requests removal; a host cancellation leaves the collection unchanged.

## Open Decisions

Persistence format, thumbnail privacy, expiry cleanup and the real restore protocol remain review items outside the control P0.

## Scenario, data, and visual tree
ResumeItem(Id,Title,Snapshot,Timestamp,ResumeToken,Expiry); tree `ContentRail→Preview/Actions`; Loading/Ready/Resuming/Expired/Error; provider declares one-shot/idempotent token.
