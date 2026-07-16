# AboutPage

## Identity

- Feature ID: `pages.about-page`
- Route: `/about`
- Route provider: `route.gallery.about`
- Status: `proposed`
- Source language: `zh-CN`

## Mission

A delivery specification for AboutPage, consuming assembly metadata + LICENSE + notice manifest without owning displayed feature implementations.

## Scope

- Entries: legal/build metadata.
- Primary action: view local legal text, copy allow-listed diagnostics, or explicitly open a link.
- Permission boundary: perform no update check or telemetry.
- Loading, empty, error, and permission/capability states are mandatory.

## Documents

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## Ownership

Only `catalog/GalleryPages/AboutPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
