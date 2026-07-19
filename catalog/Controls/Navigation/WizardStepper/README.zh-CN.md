# WizardStepper

这是中文规范源。当前实现处于 in-progress；验证策略和模板视觉仍会在后续评审继续收敛。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Navigation/WizardStepper

## 实现准备
带异步验证、恢复和线性/自由导航策略的多步骤向导指示器。验证期间防止重入；失败保持当前步骤并公告可访问错误。
