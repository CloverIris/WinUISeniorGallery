# GuideMenuExperience Specification

## Goal

Define reusable responsibilities, state, and boundaries.

## Non-goals

No implementation while proposed.

## Public API

Not locked.

## State model

Not locked.

## Template parts and visual tree

Not locked.

## Behavior and failure modes

Follow referenced contracts.

## Open Decisions

API, template parts, defaults, and performance budgets require specification review.

## Scenario, data, and visual tree
GuideNode(Id,Label,Icon,Children,Command); tree `EdgePanel→Breadcrumb→NodeList`; Closed/Root/Submenu/Executing/Error; commands close only by policy.
