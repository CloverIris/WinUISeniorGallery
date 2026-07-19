# SemanticZoomView

这是中文规范源。当前实现处于 in-progress；映射失败和转场时序仍会在后续评审中继续收敛。

## Status

in-progress / lab / P1

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Navigation/SemanticZoomView

## 实现准备
在详细视图与分组概览间切换。分组键和标题由宿主提供，控件维护焦点组与缩放状态；映射异常不得静默替换原数据。
