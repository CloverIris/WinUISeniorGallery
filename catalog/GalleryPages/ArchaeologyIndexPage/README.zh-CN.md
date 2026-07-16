# ArchaeologyIndexPage

## 身份

- Feature ID: `pages.archaeology-index-page`
- Route: `/archaeology`
- Route provider: `route.gallery.archaeology`
- Status: `proposed`
- Source language: `zh-CN`

## 使命

ArchaeologyIndexPage 的可交付页面规格，消费 Archaeology Catalog + SOURCES + license manifest，但不拥有被展示功能的实现。

## 范围

- Entries: archaeology.
- Primary action: 打开本地研究页，外链需再次显式激活.
- Permission boundary: 不下载远程历史素材或请求账户.
- Loading, empty, error, and permission/capability states are mandatory.

## 文档

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## 所有权

Only `catalog/GalleryPages/ArchaeologyIndexPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
