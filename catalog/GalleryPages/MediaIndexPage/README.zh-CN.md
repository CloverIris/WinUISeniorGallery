# MediaIndexPage

## 身份

- Feature ID: `pages.media-index-page`
- Route: `/media`
- Route provider: `route.gallery.media`
- Status: `proposed`
- Source language: `zh-CN`

## 使命

MediaIndexPage 的可交付页面规格，消费 Media catalog + related experiences，但不拥有被展示功能的实现。

## 范围

- Entries: media, experience.
- Primary action: 打开媒体规格 route.
- Permission boundary: 不请求媒体库、麦克风、摄像头或网络.
- Loading, empty, error, and permission/capability states are mandatory.

## 文档

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## 所有权

Only `catalog/GalleryPages/MediaIndexPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
