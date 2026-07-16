# PlaygroundPage

## 身份

- Feature ID: `pages.playground-page`
- Route: `/playground`
- Route provider: `route.gallery.playground`
- Status: `proposed`
- Source language: `zh-CN`

## 使命

PlaygroundPage 的可交付页面规格，消费 window-scoped Demo Registry，但不拥有被展示功能的实现。

## 范围

- Entries: registered demo.
- Primary action: 实例化白名单 demo 工厂并提交类型化属性.
- Permission boundary: 不执行任意 C#/XAML、不加载程序集、不写磁盘.
- Loading, empty, error, and permission/capability states are mandatory.

## 文档

- SPEC: information architecture, model, route, and state machine.
- DESIGN: visual tree, responsiveness, input, and accessibility.
- INTEGRATION: contracts, services, performance, and observability.
- ACCEPTANCE: Given/When/Then delivery gates.

## 所有权

Only `catalog/GalleryPages/PlaygroundPage/**` is owned by this work item. Public component APIs remain in their modern feature package.
