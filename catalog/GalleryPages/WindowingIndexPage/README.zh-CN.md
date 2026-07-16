# WindowingIndexPage

## 身份

- Feature ID: `pages.windowing-index-page`
- Route: `/windowing`
- Route provider: `route.gallery.windowing`
- Status: `proposed`
- Source language: `zh-CN`

## 使命

WindowingIndexPage 的可交付页面规格，消费 Windowing catalog + current-window capability snapshot，但不拥有被展示功能的实现。

## 范围

- Entries: windowing, behavior.
- Primary action: 先打开详情，再由详情请求窗口操作.
- Permission boundary: 不枚举其他进程窗口或请求系统权限.
- Loading, empty, error, and permission/capability states are mandatory.

## 文档

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## 所有权

Only `catalog/GalleryPages/WindowingIndexPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
