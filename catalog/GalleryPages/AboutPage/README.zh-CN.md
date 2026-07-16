# AboutPage

## 身份

- Feature ID: `pages.about-page`
- Route: `/about`
- Route provider: `route.gallery.about`
- Status: `proposed`
- Source language: `zh-CN`

## 使命

AboutPage 的可交付页面规格，消费 assembly metadata + LICENSE + notice manifest，但不拥有被展示功能的实现。

## 范围

- Entries: legal/build metadata.
- Primary action: 查看本地法律文本、复制白名单诊断或显式打开外链.
- Permission boundary: 不检查更新或发送遥测.
- Loading, empty, error, and permission/capability states are mandatory.

## 文档

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## 所有权

Only `catalog/GalleryPages/AboutPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
