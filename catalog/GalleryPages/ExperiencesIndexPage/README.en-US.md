# ExperiencesIndexPage

## Identity

- Feature ID: `pages.experiences-index-page`
- Route: `/experiences`
- Route provider: `route.gallery.experiences`
- Status: `proposed`
- Source language: `zh-CN`

## Mission

A delivery specification for ExperiencesIndexPage, consuming Feature Catalog kind=experience + dependency graph without owning displayed feature implementations.

## Scope

- Entries: experience.
- Primary action: open the experience specification and enable Run only when dependencies are complete.
- Permission boundary: start no media, multi-window flow, background task, or provider.
- Loading, empty, error, and permission/capability states are mandatory.

## Documents

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## Ownership

Only `catalog/GalleryPages/ExperiencesIndexPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
