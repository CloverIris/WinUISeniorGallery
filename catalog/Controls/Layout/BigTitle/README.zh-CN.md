# BigTitle

这是中文规范源。当前为 in-progress lab，已具备滚动驱动的大标题收缩逻辑和双呈现器模板。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Layout/BigTitle

## 实现准备
`ScrollSource` 由宿主显式提供；VerticalOffset 按 CollapseDistance 映射 0..1，Expanded/Collapsing/Collapsed 三态只影响标题视觉，不接管导航。
