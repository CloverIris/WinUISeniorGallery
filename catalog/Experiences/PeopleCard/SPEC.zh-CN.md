# PeopleCard Specification

## Goal

目标：提供宿主拥有数据的联系人摘要卡。控件只渲染显式提供的字段和动作，不读取通讯录、账号或网络。

## Non-goals

不实现联系人搜索、身份验证、消息发送、头像下载或跨窗口状态。

## Public API

`PersonCardData`（Id/DisplayName/Avatar/Fields/Actions/Presence）、`PeopleCardAction`、`Person`、`IsExpanded`、`IsActionsVisible`；方法 `InvokeAction`、`ToggleExpanded`；事件 `ActionInvoked`、`Expanded`、`Closed`。

## State model

Person 为空显示空卡；字段按宿主提供顺序渲染；重复动作 Id 只保留首项；禁用动作不触发事件；折叠只隐藏详细区域，不丢失数据。

## Template parts and visual tree

`PART_Root`、`PART_Avatar`、`PART_DisplayName`、`PART_Presence`、`PART_Fields`、`PART_Actions` 可选；缺失部件只禁用对应视觉或动作列表。

## Behavior and failure modes

非法 Person/Action 数据在 Normalize 时拒绝；宿主卸载时解除事件订阅；Automation Name 使用 `Person {DisplayName}`，不公告敏感字段。

## Open Decisions

Open Decisions：焦点恢复策略、字段脱敏策略、Avatar 加载和动作权限模型。

## 场景、数据与视觉树
模型 Person(Id,DisplayName,Avatar,Fields,Actions)；树 `Anchor→OverlayMenu/Card→Actions`；状态 Loading/Ready/Error，字段按宿主策略过滤。
