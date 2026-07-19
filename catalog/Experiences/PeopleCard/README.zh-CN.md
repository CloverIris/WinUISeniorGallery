# PeopleCard

这是中文规范源。当前为 in-progress lab，已具备本地 Person 模型、字段摘要和宿主动作事件。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Experiences/PeopleCard

## 场景准备度
控件不读取通讯录或账号数据；宿主提供 PersonCardData，控件只渲染允许字段并发出 ActionInvoked。进入 ready 前仍需锁定焦点恢复和数据最小化策略。
