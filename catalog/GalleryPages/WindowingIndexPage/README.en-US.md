# WindowingIndexPage

## Identity

- Feature ID: `pages.windowing-index-page`
- Route: `/windowing`
- Route provider: `route.gallery.windowing`
- Status: `proposed`
- Source language: `zh-CN`

## Mission

A delivery specification for WindowingIndexPage, consuming Windowing catalog + current-window capability snapshot without owning displayed feature implementations.

## Scope

- Entries: windowing, behavior.
- Primary action: open detail first and let detail request window operations.
- Permission boundary: enumerate no other process windows and request no system permission.
- Loading, empty, error, and permission/capability states are mandatory.

## Documents

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## Ownership

Only `catalog/GalleryPages/WindowingIndexPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
