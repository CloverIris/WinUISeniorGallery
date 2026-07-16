# HomePage

## 身份

- Feature ID: `pages.home-page`
- Route: `/`
- Route provider: `route.gallery.home`
- Status: `proposed`
- Source language: `zh-CN`

## 使命

HomePage 的可交付页面规格，消费 Feature Catalog + asset manifest，但不拥有被展示功能的实现。

## 范围

- Entries: control, experience, archaeology.
- Primary action: 打开精选条目的规范 route.
- Permission boundary: 不请求权限或网络.
- Loading, empty, error, and permission/capability states are mandatory.

## 文档

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## 所有权

Only `catalog/GalleryPages/HomePage/**` is owned by this work item. Public component APIs remain in their modern feature package.
