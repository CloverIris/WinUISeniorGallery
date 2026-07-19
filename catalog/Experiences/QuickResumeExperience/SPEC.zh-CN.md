# QuickResumeExperience Specification

## Goal

目标：提供按最近活动排序的十英尺会话选择器，向宿主发出恢复/移除请求，不持有真实播放器或持久化数据。

## Non-goals

不负责读取文件、生成缩略图、保存 ResumeToken、创建窗口或执行恢复。

## Public API

`QuickResumeEntry(Id, Title, Position, Duration, LastActive, Thumbnail, Tag)`；`QuickResumePicker.SetEntries` 去重并按最近活动排序；`InvokeSelected`、`RemoveSelected`、`MoveFocus`；`ItemInvoked` 和可取消 `RemoveRequested`。

## State model

`Empty`、`Ready`、`Resuming`（仅由宿主表达）和 `Removing` 请求；不可恢复或已完成条目不会触发 `ItemInvoked`。

## Template parts and visual tree

默认模板使用 `ItemsWrapGrid` 和卡片进度条；数据项由 ListView 虚拟化。缺少模板时仍可通过 API 消费确定性集合。

## Behavior and failure modes

重复 Id 保留最近活动的一项，集合最多 `MaximumItems`（默认 12）。方向键/手柄移动焦点，Enter/Space/GamepadA 触发恢复，Delete/GamepadX 触发移除请求；宿主取消移除时集合不变。

## Open Decisions

仍需评审持久化格式、缩略图隐私、过期清理和真实恢复协议；这些不属于控件 P0。

## 场景、数据与视觉树
模型 ResumeItem(Id,Title,Snapshot,Timestamp,ResumeToken,Expiry)；树 `ContentRail→Preview/Actions`；状态 Loading/Ready/Resuming/Expired/Error，Token一次性或幂等由Provider声明。
