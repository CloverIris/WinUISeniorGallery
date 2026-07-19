# MediaCenterExperience Acceptance

## Current gate

The item is `in-progress`; the synthetic local catalog is available, while visual, Automation, and runtime verification remain.

## Given / When / Then

- Given two categories, When a category is selected, Then the Grid shows only its items and enters Browse.
- Given an active item, When it is selected and Play is clicked, Then `SelectionRequested` fires without creating a player.
- Given Details is open, When Close is clicked, Then Browse is restored and `DetailsClosed` fires.
- Given an empty collection or unknown ID, When a public method is called, Then it returns false without throwing.
