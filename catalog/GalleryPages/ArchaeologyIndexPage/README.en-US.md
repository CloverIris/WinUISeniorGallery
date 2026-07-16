# ArchaeologyIndexPage

## Identity

- Feature ID: `pages.archaeology-index-page`
- Route: `/archaeology`
- Route provider: `route.gallery.archaeology`
- Status: `proposed`
- Source language: `zh-CN`

## Mission

A delivery specification for ArchaeologyIndexPage, consuming Archaeology Catalog + SOURCES + license manifest without owning displayed feature implementations.

## Scope

- Entries: archaeology.
- Primary action: open local research; external links require another explicit action.
- Permission boundary: download no remote historical asset and request no account.
- Loading, empty, error, and permission/capability states are mandatory.

## Documents

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## Ownership

Only `catalog/GalleryPages/ArchaeologyIndexPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
