# ControlsIndexPage

## 身份

- Feature ID: `pages.controls-index-page`
- Route: `/controls`
- Route provider: `route.gallery.controls`
- Status: `proposed`
- Source language: `zh-CN`

## 使命

ControlsIndexPage 的可交付页面规格，消费 Feature Catalog kind=control|behavior，但不拥有被展示功能的实现。

## 范围

- Entries: control, behavior.
- Primary action: 打开控件详情 route.
- Permission boundary: 不读取文件或搜索历史.
- Loading, empty, error, and permission/capability states are mandatory.

## 文档

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## 所有权

Only `catalog/GalleryPages/ControlsIndexPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
