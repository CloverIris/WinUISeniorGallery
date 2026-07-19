# AdaptiveGrid

这是中文规范源。当前为 in-progress，已提供确定性的布局计算辅助。

## Status

in-progress / lab / P1

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Layout/AdaptiveGrid

## 实现准备
职责是把任意项虚拟化排列为自适应等宽列；目标包 `WinUI3.Senior.Controls`。进入 ready 前须关闭 Masonry 是否属于首版的决定，默认排除。

`AdaptiveGridLayoutCalculator` 暴露相同的列数、行数和单元宽度计算，HomeScreen/Dashboard 宿主可在未创建视觉容器前预览布局。

控件在测量或排列结果真正变化时发布 `LayoutChanged`，事件参数包含不可变 `AdaptiveGridLayoutResult`；不会因为单纯的 Measure 调用重复广播。
