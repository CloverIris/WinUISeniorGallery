# ExperiencesIndexPage

## 身份

- Feature ID: `pages.experiences-index-page`
- Route: `/experiences`
- Route provider: `route.gallery.experiences`
- Status: `proposed`
- Source language: `zh-CN`

## 使命

ExperiencesIndexPage 的可交付页面规格，消费 Feature Catalog kind=experience + dependency graph，但不拥有被展示功能的实现。

## 范围

- Entries: experience.
- Primary action: 打开体验规格，依赖完整时才允许运行演示.
- Permission boundary: 不启动媒体、多窗口、后台任务或 Provider.
- Loading, empty, error, and permission/capability states are mandatory.

## 文档

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## 所有权

Only `catalog/GalleryPages/ExperiencesIndexPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
