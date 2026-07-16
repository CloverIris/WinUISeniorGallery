# HomePage

## Identity

- Feature ID: `pages.home-page`
- Route: `/`
- Route provider: `route.gallery.home`
- Status: `proposed`
- Source language: `zh-CN`

## Mission

A delivery specification for HomePage, consuming Feature Catalog + asset manifest without owning displayed feature implementations.

## Scope

- Entries: control, experience, archaeology.
- Primary action: open the featured entry's canonical route.
- Permission boundary: request no permission or network.
- Loading, empty, error, and permission/capability states are mandatory.

## Documents

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## Ownership

Only `catalog/GalleryPages/HomePage/**` is owned by this work item. Public component APIs remain in their modern feature package.
