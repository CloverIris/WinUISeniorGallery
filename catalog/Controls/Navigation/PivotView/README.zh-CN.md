# PivotView

这是中文规范源。当前实现处于 in-progress；懒加载和转场细节仍会在视觉评审中继续完善。

## Status

in-progress / lab / P1

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Navigation/PivotView

## 实现准备
轻量同级视图切换，支持键盘、RTL 镜像和可配置的触摸滑动阈值。选择一次提交一次 SelectionChanged；内容与懒加载策略由宿主拥有。
