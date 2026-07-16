# MediaIndexPage

## Identity

- Feature ID: `pages.media-index-page`
- Route: `/media`
- Route provider: `route.gallery.media`
- Status: `proposed`
- Source language: `zh-CN`

## Mission

A delivery specification for MediaIndexPage, consuming Media catalog + related experiences without owning displayed feature implementations.

## Scope

- Entries: media, experience.
- Primary action: open the media specification route.
- Permission boundary: request no media-library, microphone, camera, or network access.
- Loading, empty, error, and permission/capability states are mandatory.

## Documents

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## Ownership

Only `catalog/GalleryPages/MediaIndexPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
